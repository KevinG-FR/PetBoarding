namespace PetBoarding_Application.Payments.ProcessPayment;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Reservations;

internal sealed class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly IReservationRepository _reservationRepository;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IBasketRepository basketRepository,
        IReservationRepository reservationRepository)
    {
        _paymentRepository = paymentRepository;
        _basketRepository = basketRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var paymentId = new PaymentId(request.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
            return Result.Fail("Payment not found");

        if (!payment.IsPending())
            return Result.Fail($"Cannot process payment with status {payment.Status.Name}");

        if (request.IsSuccessful)
        {
            payment.MarkAsSuccess(request.ExternalTransactionId);
            
            var baskets = await _basketRepository.GetAllAsync(cancellationToken);
            var basket = baskets.FirstOrDefault(b => b.PaymentId == paymentId);
            
            if (basket != null)
            {
                var markPaidResult = basket.MarkAsPaidAndGetReservations();
                if (markPaidResult.IsSuccess)
                {
                    await _basketRepository.UpdateAsync(basket, cancellationToken);
                    
                    // Mark all reservations in the basket as paid
                    foreach (var reservationId in markPaidResult.Value)
                    {
                        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
                        if (reservation != null)
                        {
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
            
            if (basket != null)
            {
                var failureResult = basket.RecordPaymentFailure();
                if (failureResult.IsSuccess)
                {
                    await _basketRepository.UpdateAsync(basket, cancellationToken);
                }
            }
        }

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return Result.Ok();
    }
}