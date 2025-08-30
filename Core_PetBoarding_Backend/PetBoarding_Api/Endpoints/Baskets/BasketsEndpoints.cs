namespace PetBoarding_Api.Endpoints.Baskets;

using PetBoarding_Api.Dto.Baskets;

public static partial class BasketsEndpoints
{
    private const string RouteBase = "api/baskets";

    public static void MapBasketsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase)
            .WithTags(Tags.Baskets);

        group.MapGet("/my-basket", GetMyBasket)
            .WithName("GetMyBasket")
            .WithSummary("Get current user's basket")
            .WithDescription("Retrieves the current user's basket with all items.")
            .RequireAuthorization()            
            .Produces<BasketResponse>()
            .Produces(204)
            .Produces(401);

        group.MapPost("/items", AddItemToBasket)
            .WithName("AddItemToBasket")
            .WithSummary("Add item to basket")
            .WithDescription("Adds a prestation to the user's basket.")
            .RequireAuthorization()
            .Accepts<AddItemToBasketRequest>("application/json")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(404);

        group.MapPut("/items/{prestationId}", UpdateBasketItem)
            .WithName("UpdateBasketItem")
            .WithSummary("Update basket item quantity")
            .WithDescription("Updates the quantity of a specific item in the basket.")
            .RequireAuthorization()
            .Accepts<UpdateBasketItemRequest>("application/json")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(404);

        group.MapDelete("/items/{prestationId}", RemoveItemFromBasket)
            .WithName("RemoveItemFromBasket")
            .WithSummary("Remove item from basket")
            .WithDescription("Removes a specific item from the basket.")
            .RequireAuthorization()
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(404);

        group.MapDelete("/clear", ClearBasket)
            .WithName("ClearBasket")
            .WithSummary("Clear basket")
            .WithDescription("Removes all items from the basket.")
            .RequireAuthorization()
            .Produces(200)
            .Produces(401)
            .Produces(404);
    }
}