using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.Catalog.Common.Models;

namespace CatalogMicroservice.Service.Interfaces;

public interface ICatalogItemService
{
    Task<IEnumerable<CatalogItem>> GetItemsAsync(int pageIndex, int pageSize, int? brandId, int? typeId, CancellationToken token = default);
    Task<CatalogItem> GetItemAsync(int id, CancellationToken token = default);

    Task<(bool ok, int? id, string? error)> CreateItemAsync(CreateCatalogItem create, CancellationToken token = default);
    Task<(bool ok, string? error)>          UpdateItemAsync(CatalogItem update, CancellationToken token = default);
    Task<(bool ok, string? error)>          DeleteItemAsync(int id, CancellationToken token = default);
}
