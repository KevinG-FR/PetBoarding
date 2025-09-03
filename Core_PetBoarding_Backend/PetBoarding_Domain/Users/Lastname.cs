using FluentResults;
using Newtonsoft.Json;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Users
{
    public class Lastname : ValueObject
    {
        private const int MAX_LENGTH = 50;

        [JsonConstructor]
        private Lastname(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Lastname> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail(new NullOrEmptyError(nameof(Lastname)));

            if (value.Length > MAX_LENGTH)
                return Result.Fail(new MaxLengthError(nameof(Lastname), MAX_LENGTH));

            return Result.Ok(new Lastname(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}