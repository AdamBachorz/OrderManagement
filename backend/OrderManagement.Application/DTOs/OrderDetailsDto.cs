using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs;

public sealed record OrderDetailsDto(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    DateTimeOffset CreatedAt,
    OrderStatus Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items);