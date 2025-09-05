using Microsoft.AspNetCore.Authorization;
using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Api.Endpoints.Cache;

public static partial class CacheEndpoints
{
    private static async Task<IResult> FlushAllCache(
        ICacheService cacheService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await cacheService.FlushAllAsync(cancellationToken);
            
            if (result)
            {
                return Results.Ok(new { Message = "Cache has been flushed successfully" });
            }
            
            return Results.Problem(
                title: "Cache flush failed",
                detail: "Unable to flush the cache. Please check the logs for more information.",
                statusCode: 500);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Internal server error",
                detail: ex.Message,
                statusCode: 500);
        }
    }
}