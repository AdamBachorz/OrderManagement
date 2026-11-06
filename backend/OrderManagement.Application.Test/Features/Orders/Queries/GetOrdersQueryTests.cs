using AutoFixture;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.Tests.Features.Orders.Queries
{
    public class GetOrdersQueryTests : BaseTests
    {
        private IAppDbContext _db;
        private GetOrdersQuery _query;
        private GetOrdersQueryHandler _handler;

        public GetOrdersQueryTests() : base()
        {
            _db = _fixture.GetTestContext();           
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
            _db.Orders.AddRange(Samples());
            _query = new GetOrdersQuery(Page: 1, PageSize: 10);
            _handler = new GetOrdersQueryHandler(_db);
            // Act
            var result = await _handler.HandleAsync(_query);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<PagedResult<OrderListItemDto>>(result);
        }


    }
}
