using Api;
using Common.Protobuf;
using CsharpBeer.OrderService.Domain.Common.Interfaces;

namespace CsharpBeer.OrderService.Infrastructure.Catalog;

public class CatalogService(Api.Catalog.CatalogClient catalogClient) : ICatalogService
{
    public async Task<Beer> GetBeerByIdAsync(long beerId) => 
        (await catalogClient.GetBeerAsync(new GetBeerRequest { BeerId = beerId })).Beer;

    public async Task<List<Beer>> GetBeersByIdAsync(IEnumerable<CreateOrderItemDto> dtos)
    {
        List<Beer> beers = [];
        foreach (var dto in dtos) 
            beers.Add(await GetBeerByIdAsync(dto.BeerId));
        return beers;
    }
}