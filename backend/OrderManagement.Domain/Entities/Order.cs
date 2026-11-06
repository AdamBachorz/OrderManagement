using OrderManagement.Domain.Common;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities
{
    public class Order
    {
        public const int OrderNumberMaxLength = 32;
        public const int CustomerNameMaxLength = 200;

        private readonly List<OrderItem> _items = [];

        private Order() { }

        public Guid Id { get; private set; }
        public string OrderNumber { get; private set; } = null!;
        public string CustomerName { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }
        public OrderStatus Status { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal TotalAmount => _items.Sum(i => i.LineTotal);

        public static Order Create(
            string orderNumber,
            string customerName,
            DateTimeOffset createdAt,
            IEnumerable<OrderItemData> items)
        {
            ArgumentNullException.ThrowIfNull(items);

            // Npgsql only stores timestamptz for offset 0 (UTC)
            if (createdAt.Offset != TimeSpan.Zero)
                throw new ArgumentException("CreatedAt must be a UTC value (offset 0).", nameof(createdAt));

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = Guard.RequiredText(orderNumber, nameof(orderNumber), OrderNumberMaxLength),
                CustomerName = Guard.RequiredText(customerName, nameof(customerName), CustomerNameMaxLength),
                CreatedAt = createdAt,
                Status = OrderStatus.New
            };

            order.ReplaceItems(items);
            return order;
        }

        /// <summary>Edit (PUT): replaces the customer name and the entire list of items. Allowed only for orders in the New status.</summary>
        public void Update(string customerName, IEnumerable<OrderItemData> items)
        {
            if (Status != OrderStatus.New)
                throw new DomainException($"Only new orders can be edited (current status: {Status}).");

            var newName = Guard.RequiredText(customerName, nameof(customerName), CustomerNameMaxLength);

            ReplaceItems(items);
            CustomerName = newName;
        }

        public void Confirm()
        {
            if (Status != OrderStatus.New)
                throw new DomainException($"Only new orders can be confirmed (current status: {Status}).");

            Status = OrderStatus.Confirmed;
        }

        public void StartProcessing()
        {
            if (Status != OrderStatus.Confirmed)
                throw new DomainException($"Only confirmed orders can be moved to processing (current status: {Status}).");

            Status = OrderStatus.Processing;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Processing)
                throw new DomainException($"Only orders in processing can be completed (current status: {Status}).");

            Status = OrderStatus.Completed;
        }

        public void Cancel()
        {
            if (Status is not (OrderStatus.New or OrderStatus.Confirmed))
                throw new DomainException($"Only new or confirmed orders can be cancelled (current status: {Status}).");

            Status = OrderStatus.Cancelled;
        }

        private void ReplaceItems(IEnumerable<OrderItemData> items)
        {
            ArgumentNullException.ThrowIfNull(items);
          
            var newItems = items.Select(data => new OrderItem(Id, data)).ToList();

            if (newItems.Count == 0)
                throw new DomainException("Order must contain at least one item.");

            _items.Clear();
            _items.AddRange(newItems);
        }
    }
}
