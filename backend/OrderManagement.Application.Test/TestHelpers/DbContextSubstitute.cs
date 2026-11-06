using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Application.Tests.TestHelpers
{
    internal static class DbContextSubstitute
    {
        public static void Reset()
        {
            CreateContext().Database.EnsureDeleted();
        }

        public static AppDbContext CreateContext(bool allowLazyLoading = false)
        {
            var services = new ServiceCollection();
            services.AddTestDatabaseContext();
            services.AddLogging();
            var sp = services.BuildServiceProvider();
            var context = sp.GetRequiredService<AppDbContext>();

            return context;
        }

        public static AppDbContext GetTestContext(this IFixture fixture) => CreateContext(true);

        public static void AddTestDatabaseContext(this IFixture fixture, bool allowLazyLoading = false)
        {
            Reset();
            fixture.Register(() => CreateContext(allowLazyLoading));
        }

        public static IServiceCollection AddTestDatabaseContext(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(builder =>
            {
                builder.UseInMemoryDatabase("Dummy");
            });

            return services;
        }

    }
}
