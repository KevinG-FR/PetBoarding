using FluentResults;

namespace PetBoarding_Domain.Errors
{
    public class NullOrEmptyError : Error
    {
        public NullOrEmptyError(string parameterName) 
            : base($"Value of {parameterName} cannot be null or empty") { }        
    }
}