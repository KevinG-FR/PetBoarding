using FluentResults;

using MediatR;

namespace PetBoarding_Application.Abstractions
{
    internal interface ICommand : IRequest<Result>
    {
    }

    internal interface ICommand<TResult> : IRequest<Result<TResult>>
    {
    }
}
