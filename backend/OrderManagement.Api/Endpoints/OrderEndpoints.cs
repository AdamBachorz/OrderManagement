using Microsoft.AspNetCore.Http.HttpResults;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Features.Orders.Queries;

namespace OrderManagement.Api.Endpoints
{
    public static class OrderEndpoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public IEndpointRouteBuilder MapOrderEndpoints()
            {
                var group = app.MapGroup("/api/v1/orders").WithTags("Orders");

                group.MapGet("/", GetOrders);

                return app;
            }
        }

        private static async Task<Ok<PagedResult<OrderListItemDto>>> GetOrders(
            [AsParameters] GetOrdersQuery query,
            GetOrdersQueryHandler handler,
            CancellationToken ct)
            {
                var result = TypedResults.Ok(await handler.HandleAsync(query, ct));
                return result;
            }
    }
}
