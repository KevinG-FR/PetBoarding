using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Addresses
{
    public class Country : ValueObject
    {
        private const int MAX_LENGTH = 50;

        private Country(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Country> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail(new NullOrEmptyError(nameof(Country)));

            if (value.Length > MAX_LENGTH)
                return Result.Fail(new MaxLengthError(nameof(Country), MAX_LENGTH));

            return Result.Ok(new Country(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
