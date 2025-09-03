using FluentResults;
using Newtonsoft.Json;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Addresses;

public class City : ValueObject
{
    private const int MAX_LENGTH = 60;

    [JsonConstructor]
    private City(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<City> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(new NullOrEmptyError(nameof(City)));

        if (value.Length > MAX_LENGTH)
            return Result.Fail(new MaxLengthError(nameof(City), MAX_LENGTH));

        return Result.Ok(new City(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
