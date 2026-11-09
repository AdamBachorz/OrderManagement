using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Features.Orders.Queries;

namespace OrderManagement.Application
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplication()
            {
                services.AddScoped<GetOrdersQueryHandler>();

                return services;
            }

        }
    }
}
