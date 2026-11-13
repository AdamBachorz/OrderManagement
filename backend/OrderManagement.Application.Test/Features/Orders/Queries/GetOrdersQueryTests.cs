using OrderManagement.Application.DTOs;
using OrderManagement.Application.Exceptions;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Application.Tests.Features.Orders.Queries
{
    public class GetOrdersQueryTests : BaseTests, IAsyncLifetime
    {
        private AppDbContext _db;
        private GetOrdersQuery _query;
        private GetOrdersQueryHandler _handler;


        public async Task InitializeAsync()
        {
            Setup();
            _db = _fixture.GetTestContext();
        }

        public async Task DisposeAsync()
        {
            await _db.Database.EnsureDeletedAsync();
        }


        private static Order[] Samples() =>
        [
            TestData.OrderIn(orderNumber: "ORD-000001", customerName: "Jan Kowalski", createdAt: TestData.Now.AddDays(-3)),
            TestData.OrderIn(orderNumber: "ORD-000002", customerName: "Anna Nowak", createdAt: TestData.Now.AddDays(-2)),
            TestData.OrderIn(orderNumber: "ORD-000003", customerName: "jan nowicki", createdAt: TestData.Now.AddDays(-1)),
        ];


        [Fact]
        public async Task HandleAsync_ShouldReturnPagedResult_WhenQueryIsValid()
        {
            // Arrange
            await _db.Orders.AddRangeAsync(Samples());
            await _db.SaveChangesAsync();
            _query = new GetOrdersQuery(Page: 1, PageSize: 10);
            _handler = new GetOrdersQueryHandler(_db);
            // Act
            var result = await _handler.HandleAsync(_query);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<PagedResult<OrderListItemDto>>(result);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentNullException_WhenQueryIsNull()
        {
            // Arrange
            _handler = new GetOrdersQueryHandler(_db);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _handler.HandleAsync(null!));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task HandleAsync_ShouldThrowValidationException_WhenPageIsLessOrEqualZero(int page)
        {
            // Arrange
            _query = new GetOrdersQuery(Page: page);
            _handler = new GetOrdersQueryHandler(_db);

            // Act & Assert
            await Assert.ThrowsAsync<RequestValidationException>(
                () => _handler.HandleAsync(_query));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task HandleAsync_ShouldThrowValidationException_WhenPageSizeIsLessOrEqualZero(int pageSize)
        {
            _query = new GetOrdersQuery(PageSize: pageSize);
            _handler = new GetOrdersQueryHandler(_db);

            await Assert.ThrowsAsync<RequestValidationException>(
                () => _handler.HandleAsync(_query));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowValidationException_WhenPageSizeExceedsLimit()
        {
            _query = new GetOrdersQuery(
                PageSize: GetOrdersQuery.MaxPageSize + 1);

            _handler = new GetOrdersQueryHandler(_db);

            await Assert.ThrowsAsync<RequestValidationException>(
                () => _handler.HandleAsync(_query));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowValidationException_WhenStatusIsUnknown()
        {
            _query = new GetOrdersQuery(
                Status: (OrderStatus)999);

            _handler = new GetOrdersQueryHandler(_db);

            await Assert.ThrowsAsync<RequestValidationException>(
                () => _handler.HandleAsync(_query));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task HandleAsync_ShouldReturnAllOrders_WhenSearchIsNullOrWhiteSpace(string search)
        {
            // Arrange
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(Search: search);
            _handler = new GetOrdersQueryHandler(_db);

            // Act
            var result = await _handler.HandleAsync(_query);

            // Assert
            Assert.Equal(samples.Count(), result.TotalCount);
        }

        [Fact]
        public async Task HandleAsync_ShouldFilterByOrderNumber()
        {
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(Search: "ORD-000002");
            _handler = new GetOrdersQueryHandler(_db);

            var result = await _handler.HandleAsync(_query);

            Assert.Single(result.Items);
            Assert.Equal("ORD-000002", result.Items[0].OrderNumber);
        }

        [Fact]
        public async Task HandleAsync_ShouldFilterByCustomerName()
        {
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(Search: "Anna");
            _handler = new GetOrdersQueryHandler(_db);

            var result = await _handler.HandleAsync(_query);

            Assert.Single(result.Items);
        }

        [Fact]
        public async Task HandleAsync_ShouldSearchCaseInsensitive()
        {
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(Search: "JAN");
            _handler = new GetOrdersQueryHandler(_db);

            var result = await _handler.HandleAsync(_query);

            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnEmptyResult_WhenNothingMatches()
        {
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(Search: "XYZ");
            _handler = new GetOrdersQueryHandler(_db);

            var result = await _handler.HandleAsync(_query);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task HandleAsync_ShouldNotFilter_WhenStatusIsNone()
        {
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(Status: OrderStatus.None);
            _handler = new GetOrdersQueryHandler(_db);

            var result = await _handler.HandleAsync(_query);

            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnFirstPage()
        {
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(
                Page: 1,
                PageSize: 2);

            _handler = new GetOrdersQueryHandler(_db);

            var result = await _handler.HandleAsync(_query);

            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnSecondPage()
        {
            var samples = Samples();
            await _db.Orders.AddRangeAsync(samples);
            await _db.SaveChangesAsync();

            _query = new GetOrdersQuery(
                Page: 2,
                PageSize: 2);

            _handler = new GetOrdersQueryHandler(_db);

            var result = await _handler.HandleAsync(_query);

            Assert.Single(result.Items);
        }

    }
}
