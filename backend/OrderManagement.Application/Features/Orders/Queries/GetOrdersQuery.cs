using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Exceptions;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Queries
{
    public sealed record GetOrdersQuery(
        string Search = null,
        OrderStatus Status = OrderStatus.None,
        int Page = 1,
        int PageSize = 20)
        {
            public const int MaxPageSize = 100;
        }

    public sealed class GetOrdersQueryHandler(IAppDbContext db)
    {
        public async Task<PagedResult<OrderListItemDto>> HandleAsync(
            GetOrdersQuery query,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);
            Validate(query);

            var orders = db.Orders.AsNoTracking();

            if (query.Status is { } status && status != OrderStatus.None)
                orders = orders.Where(o => o.Status == status);

            var term = query.Search?.Trim().ToLower();
            if (!string.IsNullOrEmpty(term))
            {
                orders = orders.Where(o =>
                    o.OrderNumber.Contains(term, StringComparison.CurrentCultureIgnoreCase) ||
                    o.CustomerName.Contains(term, StringComparison.CurrentCultureIgnoreCase));
            }

            var totalCount = await orders.CountAsync(cancellationToken);

            var skip = (long)(query.Page - 1) * query.PageSize;

            List<OrderListItemDto> items = [];
            if (skip < totalCount)
            {
                items = await orders
                    .OrderByDescending(o => o.CreatedAt)
                    .ThenByDescending(o => o.Id)
                    .Skip((int)skip)
                    .Take(query.PageSize)
                    .Select(o => new OrderListItemDto(
                        o.Id,
                        o.OrderNumber,
                        o.CustomerName,
                        o.CreatedAt,
                        o.Status,
                        o.Items.Count(),
                        o.Items.Sum(i => i.Quantity * i.UnitPrice)))
                    .ToListAsync(cancellationToken);
            }

            return new PagedResult<OrderListItemDto>(items, query.Page, query.PageSize, totalCount);
        }

        //to be implemented using FluentValidator
        private static void Validate(GetOrdersQuery query)
        {
            var errors = new Dictionary<string, string[]>();

            if (query.Page < 1)
                errors["page"] = ["Page must be greater than or equal to 1."];

            if (query.PageSize is < 1 or > GetOrdersQuery.MaxPageSize)
                errors["pageSize"] = [$"Page size must be between 1 and {GetOrdersQuery.MaxPageSize}."];

            if (query.Status is { } status && !Enum.IsDefined(status))
                errors["status"] = ["Unknown order status."];

            if (errors.Count > 0)
                throw new RequestValidationException(errors);
        }
    }

}
