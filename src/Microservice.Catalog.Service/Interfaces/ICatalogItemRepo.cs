using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.Catalog.Service.Entity;

namespace Microservice.Catalog.Service.Interfaces;

public interface ICatalogItemRepo //temp
{
    Task<CatalogItemDTO?> GetAsync(int  id);
    Task<IReadOnlyCollection<CatalogItemDTO>> ListAsync();
    Task<CatalogItem> AddAsync(CatalogItem item);
    Task<CatalogItem> UpdateAsync(CatalogItem item);
    Task<bool> DeleteAsync(int id);
}
