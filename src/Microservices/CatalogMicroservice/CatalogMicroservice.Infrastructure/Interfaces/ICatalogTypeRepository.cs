using Microservice.Catalog.Common.Models;

namespace Microservice.Catalog.Infrastructure.Interfaces;

public interface ICatalogTypeRepository
{
    Task<CatalogType> GetCatalogTypeAsync(int typeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CatalogType>> GetCatalogTypesAsync(CancellationToken cancellationToken = default);
}
