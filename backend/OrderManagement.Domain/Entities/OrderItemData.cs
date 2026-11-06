namespace OrderManagement.Domain.Entities
{
    /// <summary>
    /// Data transfer object (DTO) representing the essential information of an order item.
    /// </summary>
    public readonly record struct OrderItemData(string ProductName, int Quantity, decimal UnitPrice);
}
