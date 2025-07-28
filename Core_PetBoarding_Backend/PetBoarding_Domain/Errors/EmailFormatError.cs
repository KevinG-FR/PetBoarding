using FluentResults;

namespace PetBoarding_Domain.Errors
{
    public class EmailFormatError : Error
    {
        public EmailFormatError(string email) 
            : base($"The email's format ({email}) is not valid.") {}
    }
}