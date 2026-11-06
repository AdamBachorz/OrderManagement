using OrderManagement.Domain.Common;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities
{
    public class OrderItem
    {
        public const int ProductNameMaxLength = 200;

        /// <summary>Number of decimal places consistent with the numeric(18,2) column.</summary>
        public const int PriceScale = 2;

        private OrderItem() { }

        // internal: positioning the constructor as internal allows for controlled instantiation of OrderItem objects within the assembly, while preventing external code from creating instances directly.
        // This encapsulation ensures that the creation of OrderItem objects adheres to the domain rules and validations defined in the constructor.
        internal OrderItem(Guid orderId, OrderItemData data)
        {
            var productName = Guard.RequiredText(data.ProductName, nameof(data.ProductName), ProductNameMaxLength);

            if (data.Quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            if (data.UnitPrice < 0)
                throw new DomainException("Unit price cannot be negative.");

            if (decimal.Round(data.UnitPrice, PriceScale) != data.UnitPrice)
                throw new DomainException($"Unit price can have at most {PriceScale} decimal places.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductName = productName;
            Quantity = data.Quantity;
            UnitPrice = data.UnitPrice;
        }

        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public string ProductName { get; private set; } = null!;
        public int Quantity { get; private set; }

        /// <summary>Snapshot of the price at the time of order placement.</summary>
        public decimal UnitPrice { get; private set; }

        public decimal LineTotal => Quantity * UnitPrice;
    }
}
