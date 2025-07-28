using FluentResults;

using PetBoarding_Domain.Errors.Entities;

namespace PetBoarding_Api.Extensions
{
    public static class ResultExtensions
    {
        public static IResult GetHttpResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return Results.Ok();
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        public static IResult GetHttpResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }
    }
}
