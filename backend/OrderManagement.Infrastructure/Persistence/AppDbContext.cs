using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.HasSequence<long>(PostgresOrderNumberGenerator.SequenceName);
        }
    }
}
