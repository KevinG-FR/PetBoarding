using FluentResults;

namespace PetBoarding_Domain.Errors
{
    public class MaxLengthError : Error
    {
        public MaxLengthError(string parameterName, int maxLength) 
            : base($"Value of {parameterName} must not to be greater than {maxLength}") { }
    }
}