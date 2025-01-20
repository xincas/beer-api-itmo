using CsharpBeer.OrderService.Configuration;
using CsharpBeer.OrderService.Domain.Common.Interfaces;
using CsharpBeer.OrderService.Infrastructure.Auth;
using CsharpBeer.OrderService.Infrastructure.Catalog;
using CsharpBeer.OrderService.Infrastructure.Database;
using CsharpBeer.OrderService.Infrastructure.Database.OrderItems;
using CsharpBeer.OrderService.Infrastructure.Database.Orders;
using Microsoft.EntityFrameworkCore;

namespace CsharpBeer.OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddExternalServices();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(Constants.POSTGRES_CONNECTION)
            ?? throw new ConfigurationException($"Could get ConnectionString - {Constants.POSTGRES_CONNECTION}");
        services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<OrderDbContext>());
        
        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICatalogService, CatalogService>();

        return services;
    }
}