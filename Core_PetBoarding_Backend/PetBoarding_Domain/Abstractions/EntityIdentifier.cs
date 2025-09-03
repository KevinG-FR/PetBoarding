using Newtonsoft.Json;

namespace PetBoarding_Domain.Abstractions
{
    public abstract record EntityIdentifier
    {
        public Guid Value { get; private set; }

        [JsonConstructor]
        protected EntityIdentifier(Guid value)
        {
            Value = value;
        }
    }
}