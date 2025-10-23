using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogMicroservice.Service.Interfaces;
using Microservice.Catalog.Common.Models;
using Microservice.Catalog.Infrastructure.Interfaces;

namespace CatalogMicroservice.Service.Service;

public class CatalogTypeService : ICatalogTypeService
{
    private readonly ICatalogTypeRepository? typeRepo;

    public CatalogTypeService(ICatalogTypeRepository? typeRepo)
    {
        this.typeRepo = typeRepo;
    }

    public async Task<CatalogType> GetCatalogTypeAsync(int typeId, CancellationToken token = default)
    {
        var type = await typeRepo.GetCatalogTypeAsync(typeId, token);
        return type;
    }

    public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync(CancellationToken token = default)
    {
        var types = await typeRepo.GetCatalogTypesAsync(token);
        return types;
    }
}
