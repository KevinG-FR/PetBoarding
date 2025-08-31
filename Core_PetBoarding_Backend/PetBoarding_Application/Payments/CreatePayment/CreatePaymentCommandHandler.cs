namespace PetBoarding_Application.Payments.CreatePayment;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Users;

internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Payment>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CreatePaymentCommandHandler(
        IBasketRepository basketRepository,
        IPaymentRepository paymentRepository)
    {
        _basketRepository = basketRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<Payment>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var basketId = new BasketId(request.BasketId);

        var basket = await _basketRepository.GetByIdWithItemsAsync(basketId, cancellationToken);
        if (basket is null)
            return Result.Fail<Payment>("Basket not found");

        if (basket.UserId != userId)
            return Result.Fail<Payment>("Basket does not belong to the user");

        if (!basket.RequiresPayment())
            return Result.Fail<Payment>("Basket does not require payment");

        var paymentMethod = request.Method.ToLowerInvariant() switch
        {
            "creditcard" => PaymentMethod.CreditCard,
            "paypal" => PaymentMethod.PayPal,
            "stripe" => PaymentMethod.Stripe,
            "banktransfer" => PaymentMethod.BankTransfer,
            _ => null
        };

        if (paymentMethod is null)
            return Result.Fail<Payment>("Invalid payment method");

        var payment = new Payment(
            basket.GetTotalAmount(),
            paymentMethod,
            description: request.Description);

        await _paymentRepository.AddAsync(payment, cancellationToken);

        var assignResult = basket.AssignPayment(payment.Id);
        if (assignResult.IsFailed)
        {
            await _paymentRepository.DeleteAsync(payment, cancellationToken);
            return Result.Fail<Payment>(assignResult.Errors.First().Message);
        }

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        return Result.Ok(payment);
    }
}