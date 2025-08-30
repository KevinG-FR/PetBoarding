namespace PetBoarding_Domain.Payments;

using PetBoarding_Domain.Primitives;

public sealed class PaymentMethod : Enumeration<PaymentMethod>
{
    public static readonly PaymentMethod CreditCard = new(1, nameof(CreditCard));
    public static readonly PaymentMethod PayPal = new(2, nameof(PayPal));
    public static readonly PaymentMethod Stripe = new(3, nameof(Stripe));
    public static readonly PaymentMethod BankTransfer = new(4, nameof(BankTransfer));

    public PaymentMethod(int value, string name) : base(value, name) { }
}