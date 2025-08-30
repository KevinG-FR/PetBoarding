namespace PetBoarding_Domain.Payments;

using PetBoarding_Domain.Primitives;

public sealed class PaymentStatus : Enumeration<PaymentStatus>
{
    public static readonly PaymentStatus Pending = new(1, nameof(Pending));
    public static readonly PaymentStatus Success = new(2, nameof(Success));
    public static readonly PaymentStatus Failed = new(3, nameof(Failed));
    public static readonly PaymentStatus Cancelled = new(4, nameof(Cancelled));

    public PaymentStatus(int value, string name) : base(value, name) { }
}