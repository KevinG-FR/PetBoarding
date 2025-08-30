namespace PetBoarding_Api.Endpoints.Payments;

using PetBoarding_Api.Dto.Payments;

public static partial class PaymentsEndpoints
{
    private const string RouteBase = "api/payments";

    public static void MapPaymentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase)
            .WithTags(Tags.Payments);

        group.MapPost("", CreatePayment)
            .WithName("CreatePayment")
            .WithSummary("Create payment for basket")
            .WithDescription("Creates a new payment for the specified basket.")
            .RequireAuthorization()
            .Accepts<CreatePaymentRequest>("application/json")
            .Produces<PaymentResponse>(201)
            .Produces(400)
            .Produces(401)
            .Produces(404);

        group.MapPost("/{paymentId}/process", ProcessPayment)
            .WithName("ProcessPayment")
            .WithSummary("Process payment result")
            .WithDescription("Processes the result of a payment (success or failure).")
            .RequireAuthorization()
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(404);
    }
}