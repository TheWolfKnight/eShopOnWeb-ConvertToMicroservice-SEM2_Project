using Microservice.Catalog.Infrastructure.Interfaces;
using Microservice.Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Catalog.Infrastructure.DependencyInjection;

public static class CatalogTypeDependencyInjection
{
    public static IServiceCollection AddCatalogTypeRepository(this IServiceCollection @this, string connectionString)
    {
        @this.AddKeyedSingleton<string>(CatalogTypeRepository.CONNECTION_STRING_KEY, connectionString);
        @this.AddScoped<ICatalogTypeRepository, CatalogTypeRepository>();

        return @this;
    }
}
