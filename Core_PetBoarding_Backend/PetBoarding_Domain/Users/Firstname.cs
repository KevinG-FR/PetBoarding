using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Users
{
    public class Firstname : ValueObject
    {
        private const int MAX_LENGTH = 50;

        private Firstname(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Firstname> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail(new NullOrEmptyError(nameof(Firstname)));

            if (value.Length > MAX_LENGTH)
                return Result.Fail(new MaxLengthError(nameof(Firstname), MAX_LENGTH));

            return Result.Ok(new Firstname(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}