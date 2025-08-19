using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Users
{
    public class PhoneNumber : ValueObject
    {
        private const int MAX_LENGTH = 15;

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail(new NullOrEmptyError(nameof(PhoneNumber)));

            if (value.Length > MAX_LENGTH)
                return Result.Fail(new MaxLengthError(nameof(PhoneNumber), MAX_LENGTH));

            return Result.Ok(new PhoneNumber(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}