using BaseServerProject.Core.Entities;

namespace BaseServerProject.Application.Common.Interfaces;

public interface IProductRepository
{
    IQueryable<Product> GetQueryable();
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> GetByCodeAsync(string productCode, CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default); 
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}