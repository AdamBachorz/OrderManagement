using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs;

public sealed record OrderListItemDto(
Guid Id,
string OrderNumber,
string CustomerName,
DateTimeOffset CreatedAt,
OrderStatus Status,
int ItemsCount,
decimal TotalAmount);
