namespace PetBoarding_Domain.Abstractions
{
    public abstract record EntityIdentifier
    {
        public Guid Value { get; private set; }

        protected EntityIdentifier(Guid value)
        {
            Value = value;
        }
    }
}