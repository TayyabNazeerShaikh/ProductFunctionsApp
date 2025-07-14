using System.Collections.Concurrent;
using ProductFunctionsApp.Application.Interfaces;
using ProductFunctionsApp.Domain.Entities;

namespace ProductFunctionsApp.Infrastructure.Repositories;

// NOTE: A singleton lifetime for this service is required for an in-memory repository.
// For a real application, this would be a database repository (e.g., using EF Core, Dapper, or Cosmos DB SDK).
public class InMemoryProductRepository : IProductRepository
{
    private static readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task<Product> AddAsync(Product product)
    {
        _products[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_products.TryRemove(id, out _));
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_products.Values.OrderBy(p => p.CreatedAt));
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(Guid id, Product product)
    {
        if (!_products.ContainsKey(id))
        {
            return Task.FromResult<Product?>(null);
        }
        _products[id] = product;
        return Task.FromResult<Product?>(product);
    }
}
