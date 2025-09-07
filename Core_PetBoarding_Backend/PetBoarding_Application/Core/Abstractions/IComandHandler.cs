using FluentResults;

using MediatR;

namespace PetBoarding_Application.Core.Abstractions
{
    internal interface ICommandHandler<TCommand>
        : IRequestHandler<TCommand, Result>
            where TCommand : ICommand
    {
    }

    internal interface ICommandHandler<TCommand, TResult>
        : IRequestHandler<TCommand, Result<TResult>>
            where TCommand : ICommand<TResult>
    {
    }
}
