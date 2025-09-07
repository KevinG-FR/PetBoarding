using FluentResults;

using MediatR;

namespace PetBoarding_Application.Core.Abstractions
{
    internal interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
