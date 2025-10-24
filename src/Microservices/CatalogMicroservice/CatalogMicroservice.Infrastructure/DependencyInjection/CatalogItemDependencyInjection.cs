using CatalogMicroservice.Infrastructure.Interfaces;
using CatalogMicroservice.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogMicroservice.Infrastructure.DependencyInjection;

public static class CatalogItemRepositoryDependencyInjection
{
    public static IServiceCollection AddCatalogItemRepository(this IServiceCollection @this, string connectionString)
    {
        @this.AddKeyedScoped<string>(CatalogItemRepository.CONNECTION_STRING_KEY, (serviceProvider, key) => connectionString);
        @this.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
        @this.AddScoped<CatalogItemRepository>();

        return @this;
    }

    public static IServiceScope SetupCatalogDatabase(this IServiceScope @this)
    {
        IServiceProvider provider = @this.ServiceProvider;
        CatalogItemRepository? repository = provider.GetService<CatalogItemRepository>();

        if (repository is null)
            return @this;

        Task ensureDbTask = repository.EnsureDbExistsAsync();
        ensureDbTask.Wait();

        return @this;
    }
}
