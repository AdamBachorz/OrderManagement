using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Abstractions.Persistence
{
    public interface IAppDbContext
    {
        DbSet<Order> Orders { get; }
    }
}
