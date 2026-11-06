using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddInfrastructure(string connectionString)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

                services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
                services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
                services.AddScoped<IOrderNumberGenerator, PostgresOrderNumberGenerator>();

                return services;
            }
        }
        
    }
}
