using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Addresses
{
    public class PostalCode : ValueObject
    {
        private const int MAX_LENGTH = 10;

        private PostalCode(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<PostalCode> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail(new NullOrEmptyError(nameof(PostalCode)));

            if (value.Length > MAX_LENGTH)
                return Result.Fail(new MaxLengthError(nameof(PostalCode), MAX_LENGTH));

            return Result.Ok(new PostalCode(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
