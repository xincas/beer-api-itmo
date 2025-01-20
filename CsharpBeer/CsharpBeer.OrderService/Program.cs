using Api;
using CsharpBeer.OrderService;
using CsharpBeer.OrderService.Configuration;
using CsharpBeer.OrderService.Infrastructure;
using CsharpBeer.OrderService.Services;
using AuthC = Auth.Auth;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    builder.Services.AddGrpc(op => 
    {
        op.EnableDetailedErrors = true;
    });
    
    var grpcConnections = builder.Configuration.GetSection(Constants.GRPC_SECTION).Get<GrpcConnection>() ?? throw new ConfigurationException($"Could not map section {Constants.GRPC_SECTION}");
    builder.Services.AddGrpcClient<Catalog.CatalogClient>(f => f.Address = new Uri(grpcConnections.CatalogAddress ?? ""));
    builder.Services.AddGrpcClient<AuthC.AuthClient>(f => f.Address = new Uri(grpcConnections.IdentityAddress ?? ""));
    
    builder.Services.AddInfrastructure(builder.Configuration);
}

var app = builder.Build();

{
    app.MapGrpcService<OrderService>();
}

app.Run();