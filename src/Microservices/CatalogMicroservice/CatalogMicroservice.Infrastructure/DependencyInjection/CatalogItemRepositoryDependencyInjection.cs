using Microservice.Catalog.Infrastructure.Interfaces;
using Microservice.Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Catalog.Infrastructure.DependencyInjection;

public static class CatalogItemRepositoryDependencyInjection
{
    public static IServiceCollection AddCatalogItemRepository(this IServiceCollection @this, string connectionString)
    {
        @this.AddKeyedScoped<string>(CatalogItemRepository.CONNECTION_STRING_KEY, (serviceProvider, key) => connectionString);
        @this.AddScoped<ICatalogItemRepository, CatalogItemRepository>();

        return @this;
    }
}
