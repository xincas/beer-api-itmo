namespace CsharpBeer.OrderService.Configuration;

public record GrpcConnection(
    string? CatalogAddress,
    string? IdentityAddress);