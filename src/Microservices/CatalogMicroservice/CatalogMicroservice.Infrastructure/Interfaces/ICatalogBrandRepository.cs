using Microservice.Catalog.Common.Models;

namespace Microservice.Catalog.Infrastructure.Interfaces;

public interface ICatalogBrandRepository
{
    Task<CatalogBrand?> GetBrandByIdAsync(int brandId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CatalogBrand>> GetBrandsAsync(CancellationToken cancellationToken = default);
}
