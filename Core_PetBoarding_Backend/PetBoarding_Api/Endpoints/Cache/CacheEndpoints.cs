using Microsoft.AspNetCore.Authorization;
using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Api.Endpoints.Cache;

public static partial class CacheEndpoints
{
    private const string RouteBase = "api/cache";

    public static void MapCacheEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase);

        group.MapDelete("/flush", FlushAllCache)
            .WithName("FlushAllCache")
            .WithSummary("Flush all cache entries")
            .WithDescription("Clears all entries from the cache system.")
            .Produces(200)
            .Produces(500);
    }
}