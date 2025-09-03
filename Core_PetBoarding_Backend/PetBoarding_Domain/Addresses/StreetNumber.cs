using FluentResults;
using Newtonsoft.Json;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Addresses;

public class StreetNumber : ValueObject
{
    private const int MAX_LENGTH = 10;

    [JsonConstructor]
    private StreetNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<StreetNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(new NullOrEmptyError(nameof(StreetNumber)));

        if (value.Length > MAX_LENGTH)
            return Result.Fail(new MaxLengthError(nameof(StreetNumber), MAX_LENGTH));

        return Result.Ok(new StreetNumber(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
