using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.Catalog.Common.Models;

namespace CatalogMicroservice.Service.Interfaces;

public interface ICatalogTypeService
{
    Task<CatalogType> GetCatalogTypeAsync(int typeId, CancellationToken token = default);
    Task<IEnumerable<CatalogType>> GetCatalogTypesAsync(CancellationToken token = default);
}
