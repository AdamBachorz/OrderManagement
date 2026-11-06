using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Tests.TestHelpers;

internal static class TestData
{
    public static readonly DateTimeOffset Now = new(2026, 1, 15, 10, 30, 0, TimeSpan.Zero);

    public static OrderItemData Item(string productName = "Keyboard", int quantity = 1, decimal unitPrice = 10m)
        => new(productName, quantity, unitPrice);

    public static Order OrderIn(
        OrderStatus status = OrderStatus.New,
        string orderNumber = "ORD-000001",
        string customerName = "Jan Kowalski",
        DateTimeOffset? createdAt = null,
        OrderItemData[]? items = null)
    {
        var order = Order.Create(orderNumber, customerName, createdAt ?? Now, items ?? [Item()]);

        switch (status)
        {
            case OrderStatus.New:
                break;
            case OrderStatus.Confirmed:
                order.Confirm();
                break;
            case OrderStatus.Processing:
                order.Confirm();
                order.StartProcessing();
                break;
            case OrderStatus.Completed:
                order.Confirm();
                order.StartProcessing();
                order.Complete();
                break;
            case OrderStatus.Cancelled:
                order.Cancel();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        return order;
    }
}
