using FluentResults;

using MediatR;

namespace PetBoarding_Application.Abstractions
{
    internal interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
