using System.Text.Json;
using Common.Protobuf;
using CsharpBeer.OrderService;
using CsharpBeer.OrderService.Domain.Common.Interfaces;
using CsharpBeer.OrderService.Domain.Orders;
using CsharpBeer.OrderService.Infrastructure.Database;
using DotNet.Testcontainers.Builders;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.PostgreSql;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CsharpBeer.IntegrationTests;

public class OrderMicroserviceFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private const string DatabaseImage = "postgres:latest";
    private const string DatabaseName = "Orders";
    private const string DatabaseUserName = "user";
    private const string DatabaseUserPassword = "somepassword";
    private const int DatabaseInternalPort = 5432;
    private const int DatabaseExternalPort = 5555;
    
    private static readonly PostgreSqlContainer _database = new PostgreSqlBuilder()
        .WithImage(DatabaseImage)
        .WithPortBinding(DatabaseExternalPort,DatabaseInternalPort)
        .WithDatabase(DatabaseName)
        .WithUsername(DatabaseUserName)
        .WithPassword(DatabaseUserPassword)
        .WithWaitStrategy(Wait
            .ForUnixContainer()
            .UntilPortIsAvailable(DatabaseInternalPort))
        .Build();

    private const string ordersSeedPath = "orders.json";
    private const string orderItemsSeedPath = "orderitems.json";
    private List<Order>? _orders { get; set; }
    private List<OrderItem>? _items { get; set; }
    
    public readonly Mock<IAuthService> MockAuthService = new();
    public readonly Mock<ICatalogService> MockCatalogService = new();
    public OrderServiceGrpc.OrderServiceGrpcClient GrpcClient => _grpcClient;
    private OrderServiceGrpc.OrderServiceGrpcClient _grpcClient = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(OrderDbContext));
            services.RemoveAll(typeof(IAuthService));
            services.RemoveAll(typeof(ICatalogService));
            services.RemoveAll(typeof(DbContextOptions<OrderDbContext>));

            var connection = _database.GetConnectionString();
            services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(connection));
            services.AddScoped<IAuthService>(_ => MockAuthService.Object);
            services.AddScoped<ICatalogService>(_ => MockCatalogService.Object);
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        var options = new GrpcChannelOptions { HttpHandler = Server.CreateHandler() };
        var channel = GrpcChannel.ForAddress(Server.BaseAddress, options);
        _grpcClient = new OrderServiceGrpc.OrderServiceGrpcClient(channel);
        
        await using var orderStream = File.OpenRead(ordersSeedPath);
        await using var orderItemsStream = File.OpenRead(orderItemsSeedPath);
        _orders = await JsonSerializer.DeserializeAsync<List<Order>>(orderStream);
        _items = await JsonSerializer.DeserializeAsync<List<OrderItem>>(orderItemsStream);
    }

    public async Task DbCallAsync(Func<OrderDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        await action(dbContext);
    }
    public async Task SeedDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        
        await ClearAllDatabase(context);
        
        context.Orders.AddRange(_orders);
        context.OrderItems.AddRange(_items);
        await context.SaveChangesAsync();
        
        await context.Database.ExecuteSqlRawAsync("SELECT setval('\"Orders_OrderId_seq\"', 7, true);");
    }

    private async Task ClearAllDatabase(OrderDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _database.StopAsync();
        await base.DisposeAsync();
    }
}