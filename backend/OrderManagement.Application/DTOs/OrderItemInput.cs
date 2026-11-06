namespace OrderManagement.Application.DTOs;

public sealed record OrderItemInput(string? ProductName, int Quantity, decimal UnitPrice);

