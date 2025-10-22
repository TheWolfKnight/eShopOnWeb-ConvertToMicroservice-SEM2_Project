using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.Catalog.Service.Entity;

namespace Microservice.Catalog.Service.Interfaces;

public interface ICatalogLookup
{
    Task<IReadOnlyCollection<CatalogBrand>> BrandsAsync();
    Task<IReadOnlyCollection<CatalogType>>  TypesAsync();
    Task<CatalogBrand> GetBrandAsync(int id);
    Task<CatalogItem>  GetItemAsync(int id);
}
