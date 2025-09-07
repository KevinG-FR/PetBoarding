using FluentResults;

using MediatR;

namespace PetBoarding_Application.Core.Abstractions
{
    internal interface IQueryHandler<TQuery, TResult>
        : IRequestHandler<TQuery, Result<TResult>>
            where TQuery : IQuery<TResult>
    {
    }
}
