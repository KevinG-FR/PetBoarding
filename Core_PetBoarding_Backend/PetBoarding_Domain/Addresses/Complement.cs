using FluentResults;
using Newtonsoft.Json;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Addresses;

public class Complement : ValueObject
{
    private const int MAX_LENGTH = 100;

    [JsonConstructor]
    private Complement(string? value)
    {
        Value = value;
    }

    public string? Value { get; }

    public static Result<Complement> Create(string? value)
    {
        if (value != null && value.Length > MAX_LENGTH)
            return Result.Fail(new MaxLengthError(nameof(Complement), MAX_LENGTH));

        return Result.Ok(new Complement(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value ?? string.Empty;
    }
}
