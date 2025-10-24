using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogMicroservice.Service.Interfaces;
using Microservice.Catalog.Common.Models;
using Microservice.Catalog.Infrastructure.Interfaces;

namespace CatalogMicroservice.Service.Service;
public class CatalogBrandService : ICatalogBrandService
{
    private readonly ICatalogBrandRepository? brandRepo;

    public CatalogBrandService(ICatalogBrandRepository? brandRepo)
    {
        this.brandRepo = brandRepo;
    }

    public Task<CatalogBrand> GetBrandByIdAsync(int brandId, CancellationToken cancellationToken = default)
    {
        var brand = GetBrandByIdAsync(brandId, cancellationToken);
        return brand;
    }

    public async Task<IEnumerable<CatalogBrand>> GetBrandsAsync(CancellationToken token = default)
    {
        var brands = await brandRepo.GetBrandsAsync(token);
        return brands;
    }
}
