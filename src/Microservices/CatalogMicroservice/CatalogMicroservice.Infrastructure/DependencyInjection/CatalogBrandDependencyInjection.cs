using CatalogMicroservice.Infrastructure.Interfaces;
using CatalogMicroservice.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogMicroservice.Infrastructure.DependencyInjection;

public static class CatalogBrandDependencyInjection
{
    public static IServiceCollection AddCatalogBrandRepository(this IServiceCollection @this, string connectionString)
    {
        @this.AddKeyedScoped<string>(CatalogBrandRepository.CONNECTION_STRING_KEY, (_, _) => connectionString);
        @this.AddScoped<ICatalogBrandRepository, CatalogBrandRepository>();

        return @this;
    }
}
