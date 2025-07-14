using ProductFunctionsApp.Domain.Entities;

namespace ProductFunctionsApp.Application.Interfaces;

public interface IProductRepository
{
    Task<Product> AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> UpdateAsync(Guid id, Product product);
    Task<bool> DeleteAsync(Guid id);
}
