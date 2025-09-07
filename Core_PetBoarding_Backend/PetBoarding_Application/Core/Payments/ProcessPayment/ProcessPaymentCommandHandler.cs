namespace PetBoarding_Application.Core.Payments.ProcessPayment;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Planning.ReleaseSlots;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

internal sealed class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IReleaseSlotService _releaseSlotService;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IBasketRepository basketRepository,
        IReservationRepository reservationRepository,
        IReleaseSlotService releaseSlotService,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _basketRepository = basketRepository;
        _reservationRepository = reservationRepository;
        _releaseSlotService = releaseSlotService;
        _logger = logger;
    }

    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var paymentId = new PaymentId(request.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
            return Result.Fail("Payment not found");

        if (!payment.IsPending())
            return Result.Fail($"Cannot process payment with status {payment.Status.Name}");

        if (request.IsSuccessful)
        {
            var userId = new UserId(request.UserId);
            payment.MarkAsSuccess(userId, request.ExternalTransactionId);
            
            var baskets = await _basketRepository.GetAllAsync(cancellationToken);
            var basket = baskets.FirstOrDefault(b => b.PaymentId == paymentId);
            
            if (basket is not null)
            {
                var markPaidResult = basket.MarkAsPaidAndGetReservations();
                if (markPaidResult.IsSuccess)
                {
                    await _basketRepository.UpdateAsync(basket, cancellationToken);
                    
                    // Mark all reservations in the basket as paid
                    foreach (var reservationId in markPaidResult.Value)
                    {
                        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
                        if (reservation is not null)
                        {
                            // Le TotalPrice doit maintenant toujours être défini lors de la création de la réservation
                            if (reservation.TotalPrice == null)
                            {
                                _logger.LogWarning("Reservation {ReservationId} has null TotalPrice, this should not happen", reservationId.Value);
                            }

                            reservation.MarkAsPaid();
                            await _reservationRepository.UpdateAsync(reservation, cancellationToken);
                        }
                    }
                }
            }
        }
        else
        {
            payment.MarkAsFailed(request.FailureReason ?? "Payment processing failed");
            
            var baskets = await _basketRepository.GetAllAsync(cancellationToken);
            var basket = baskets.FirstOrDefault(b => b.PaymentId == paymentId);
            
            if (basket is not null)
            {
                // Vérifier si c'est le dernier échec qui va causer l'annulation
                var wasBasketCancelled = basket.PaymentFailureCount + 1 >= 3; // MAX_PAYMENT_FAILURES
                
                var failureResult = basket.RecordPaymentFailure();
                if (failureResult.IsSuccess)
                {
                    await _basketRepository.UpdateAsync(basket, cancellationToken);
                    
                    // Si le panier a été annulé à cause de trop d'échecs, libérer tous les créneaux
                    if (wasBasketCancelled && basket.Status == BasketStatus.Cancelled)
                    {
                        _logger.LogWarning("Basket {BasketId} was cancelled due to payment failures. Releasing all slots.", basket.Id.Value);
                        
                        var reservationIds = basket.GetReservationIds().ToList();
                        var totalReleasedSlots = 0;
                        
                        foreach (var reservationId in reservationIds)
                        {
                            var releaseResult = await _releaseSlotService.ReleaseReservationSlotsAsync(reservationId, cancellationToken);
                            if (releaseResult.IsSuccess)
                            {
                                totalReleasedSlots += releaseResult.Value;
                            }
                            else
                            {
                                _logger.LogError("Failed to release slots for reservation {ReservationId} from cancelled basket {BasketId}: {Errors}",
                                    reservationId.Value, basket.Id.Value, string.Join(", ", releaseResult.Errors.Select(e => e.Message)));
                            }
                        }
                        
                        _logger.LogInformation("Released {SlotCount} slots from cancelled basket {BasketId}", 
                            totalReleasedSlots, basket.Id.Value);
                    }
                }
            }
        }

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return Result.Ok();
    }
}