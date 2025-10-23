using Microservice.Catalog.Infrastructure.Interfaces;
using Microservice.Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Catalog.Infrastructure.DependencyInjection;

public static class CatalogBrandDependencyInjection
{
    public static IServiceCollection AddCatalogBrandRepository(this IServiceCollection @this, string connectionString)
    {
        @this.AddKeyedScoped<string>(CatalogBrandRepository.CONNECTION_STRING_KEY, (_, _) => connectionString);
        @this.AddScoped<ICatalogBrandRepository, ICatalogBrandRepository>();

        return @this;
    }
}
