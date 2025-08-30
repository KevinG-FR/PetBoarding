namespace PetBoarding_Domain.Baskets;

using PetBoarding_Domain.Primitives;

public sealed class BasketStatus : Enumeration<BasketStatus>
{
    public static readonly BasketStatus Created = new(1, nameof(Created));
    public static readonly BasketStatus Cancelled = new(2, nameof(Cancelled));
    public static readonly BasketStatus PaymentFailure = new(3, nameof(PaymentFailure));
    public static readonly BasketStatus Paid = new(4, nameof(Paid));

    public BasketStatus(int value, string name) : base(value, name) { }
}