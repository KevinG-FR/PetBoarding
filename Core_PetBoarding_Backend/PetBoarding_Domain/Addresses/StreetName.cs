using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Addresses
{
    public class StreetName : ValueObject
    {
        private const int MAX_LENGTH = 100;

        private StreetName(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<StreetName> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail(new NullOrEmptyError(nameof(StreetName)));

            if (value.Length > MAX_LENGTH)
                return Result.Fail(new MaxLengthError(nameof(StreetName), MAX_LENGTH));

            return Result.Ok(new StreetName(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
