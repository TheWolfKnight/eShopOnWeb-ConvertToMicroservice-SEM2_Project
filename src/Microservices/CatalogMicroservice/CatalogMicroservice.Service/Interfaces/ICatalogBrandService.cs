using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.Catalog.Common.Models;

namespace CatalogMicroservice.Service.Interfaces;
public interface ICatalogBrandService
{
    Task<CatalogBrand> GetBrandByIdAsync(int brandId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CatalogBrand>> GetBrandsAsync(CancellationToken token = default);
}
