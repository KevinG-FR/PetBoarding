using System.Text.RegularExpressions;

using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Users
{
    public class Email : ValueObject
    {
        private const int MAX_LENGTH = 128;
        private const string REGEX_EMAIL = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Email> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail(new NullOrEmptyError(nameof(Email)));

            if (value.Length > MAX_LENGTH)
                return Result.Fail(new MaxLengthError(nameof(Email), MAX_LENGTH));

            if (CheckIfEmailFormatIsNotValid(value))
                return Result.Fail(new EmailFormatError(value));

            return Result.Ok(new Email(value));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }

        private static bool CheckIfEmailFormatIsNotValid(string email)
        {
            return !Regex.IsMatch(email, REGEX_EMAIL);
        }
    }
}