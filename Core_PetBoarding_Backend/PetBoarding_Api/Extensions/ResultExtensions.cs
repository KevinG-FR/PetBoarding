using FluentResults;

using PetBoarding_Domain.Errors.Entities;

namespace PetBoarding_Api.Extensions
{
    public static class ResultExtensions
    {
        /// <summary>
        /// Méthode helper pour créer le bon type de résultat selon le status code
        /// </summary>
        private static IResult CreateSuccessResult<T>(T value, SuccessStatusCode? statusCode = null)
        {
            return (statusCode ?? SuccessStatusCode.Ok) switch
            {
                SuccessStatusCode.Ok => Results.Ok(value),
                SuccessStatusCode.Created => Results.Created(string.Empty, value),
                SuccessStatusCode.Accepted => Results.Accepted(string.Empty, value),
                SuccessStatusCode.NoContent => Results.NoContent(),
                _ => Results.Ok(value)
            };
        }

        /// <summary>
        /// Méthode helper pour créer le bon type de résultat avec location pour Created
        /// </summary>
        private static IResult CreateSuccessResult<T>(T value, string? location, SuccessStatusCode? statusCode = null)
        {
            return (statusCode ?? SuccessStatusCode.Ok) switch
            {
                SuccessStatusCode.Ok => Results.Ok(value),
                SuccessStatusCode.Created => Results.Created(location ?? string.Empty, value),
                SuccessStatusCode.Accepted => Results.Accepted(location ?? string.Empty, value),
                SuccessStatusCode.NoContent => Results.NoContent(),
                _ => Results.Ok(value)
            };
        }

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

        /// <summary>
        /// Extension générique pour mapper une entité vers un DTO et retourner un HttpResult
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto>(this Result<TEntity> result, Func<TEntity, TDto> mapper)
        {
            if (result.IsSuccess)
            {
                var dto = mapper(result.Value);
                return Results.Ok(dto);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        /// <summary>
        /// Extension générique pour mapper une entité vers un DTO puis vers un objet de réponse structuré
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto, TResponse>(
            this Result<TEntity> result, 
            Func<TEntity, TDto> mapper,
            Func<TDto, TResponse> responseBuilder,
            SuccessStatusCode? successStatusCode = null)
        {
            if (result.IsSuccess)
            {
                var dto = mapper(result.Value);
                var response = responseBuilder(dto);
                return CreateSuccessResult(response, successStatusCode);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        /// <summary>
        /// Extension générique pour mapper une entité vers un DTO puis vers un objet de réponse structuré avec location URI
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto, TResponse>(
            this Result<TEntity> result, 
            Func<TEntity, TDto> mapper,
            Func<TDto, TResponse> responseBuilder,
            Func<TEntity, string> locationBuilder,
            SuccessStatusCode? successStatusCode = null)
        {
            if (result.IsSuccess)
            {
                var dto = mapper(result.Value);
                var response = responseBuilder(dto);
                var location = locationBuilder(result.Value);
                return CreateSuccessResult(response, location, successStatusCode);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        /// <summary>
        /// Extension générique pour mapper une collection d'entités vers une collection de DTOs
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto>(this Result<IReadOnlyList<TEntity>> result, Func<TEntity, TDto> mapper)
        {
            if (result.IsSuccess)
            {
                var dtos = result.Value.Select(mapper).ToList();
                return Results.Ok(dtos);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        /// <summary>
        /// Extension générique pour mapper une List d'entités vers une collection de DTOs
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto>(this Result<List<TEntity>> result, Func<TEntity, TDto> mapper)
        {
            if (result.IsSuccess)
            {
                var dtos = result.Value.Select(mapper).ToList();
                return Results.Ok(dtos);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        /// <summary>
        /// Extension pour mapper une collection vers un objet de réponse structuré
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto, TResponse>(
            this Result<IReadOnlyList<TEntity>> result, 
            Func<TEntity, TDto> mapper,
            Func<IReadOnlyList<TDto>, TResponse> responseBuilder)
        {
            if (result.IsSuccess)
            {
                var dtos = result.Value.Select(mapper).ToList();
                var response = responseBuilder(dtos);
                return Results.Ok(response);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        /// <summary>
        /// Extension pour mapper une List vers un objet de réponse structuré
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto, TResponse>(
            this Result<List<TEntity>> result, 
            Func<TEntity, TDto> mapper,
            Func<IReadOnlyList<TDto>, TResponse> responseBuilder)
        {
            if (result.IsSuccess)
            {
                var dtos = result.Value.Select(mapper).ToList();
                var response = responseBuilder(dtos);
                return Results.Ok(response);
            }

            var notFoundError = result.Errors.FirstOrDefault(x => x is INotFoundError)?.Message;

            if (result.IsFailed && notFoundError is not null)
            {
                return Results.NotFound(notFoundError);
            }

            return Results.BadRequest(result.Errors);
        }

        /// <summary>
        /// Extension pour mapper une IEnumerable vers un objet de réponse structuré
        /// </summary>
        public static IResult GetHttpResult<TEntity, TDto, TResponse>(
            this Result<IEnumerable<TEntity>> result, 
            Func<TEntity, TDto> mapper,
            Func<IReadOnlyList<TDto>, TResponse> responseBuilder)
        {
            if (result.IsSuccess)
            {
                var dtos = result.Value.Select(mapper).ToList();
                var response = responseBuilder(dtos);
                return Results.Ok(response);
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
