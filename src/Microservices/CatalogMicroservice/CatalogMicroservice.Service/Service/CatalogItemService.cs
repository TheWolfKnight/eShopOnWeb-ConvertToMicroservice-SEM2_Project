using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogMicroservice.Service.Interfaces;
using Microservice.Catalog.Common.Models;
using Microservice.Catalog.Infrastructure.Interfaces;

namespace CatalogMicroservice.Service;

internal class CatalogItemService : ICatalogItemService
{
    private readonly ICatalogItemRepository? itemRepo;
    private readonly ICatalogBrandService? brandRepo;
    private readonly ICatalogTypeService? typeRepo;

    public CatalogItemService(
        ICatalogItemRepository? itemRepo, ICatalogBrandService? brandRepo, ICatalogTypeService? typeRepo)
    {
        this.itemRepo = itemRepo;
        this.brandRepo = brandRepo;
        this.typeRepo = typeRepo;
    }

    public async Task<IEnumerable<CatalogItem>> GetItemsAsync(int pageIndex, int pageSize, int? brandId, int? typeId, CancellationToken token = default)
    {
        if (pageIndex < 0) { pageIndex = 0; }
        if (pageSize <= 0) { pageSize = 10; }

        var page = await itemRepo.GetItemPageAsync(pageIndex, pageSize, brandId, typeId, token);

        return page;
    }

    public async Task<CatalogItem> GetItemAsync(int id, CancellationToken token = default)
    {
        return await itemRepo.GetItemAsync(id, token);
    }

    public async Task<bool> CreateItemAsync(CreateCatalogItem item, CancellationToken token = default)
    {
        var brand = await brandRepo.GetBrandByIdAsync(item.CatalogBrandId, token);
        var type  = await typeRepo.GetCatalogTypeAsync(item.CatalogTypeId, token);
        if (brand is null || type is null) { return false; }

        var created = await itemRepo.CreateItemAsync(item, token);
        return true;
    }

    public async Task<bool> UpdateItemAsync(CatalogItem updateItem, CancellationToken token = default)
    {
        var existing = await itemRepo.GetItemAsync(updateItem.Id, token);
        if (existing is null) { return false; }

        var brand = await brandRepo.GetBrandByIdAsync(updateItem.CatalogBrandId, token);
        var type  = await typeRepo.GetCatalogTypeAsync(updateItem.CatalogTypeId, token);
        if (brand is null || type is null) { return false; }

        existing.Name           = updateItem.Name;
        existing.Description    = updateItem.Description;
        existing.Price          = updateItem.Price;
        existing.PictureUri     = updateItem.PictureUri;
        existing.CatalogBrandId = updateItem.CatalogBrandId;
        existing.CatalogTypeId  = updateItem.CatalogTypeId;

        await itemRepo.UpdateItemAsync(existing, token);
        return true;
    }

    public async Task<bool> DeleteItemAsync(int id, CancellationToken token = default)
    {
        var exists = await itemRepo.GetItemAsync(id, token);
        if (exists is null) { return false; }

        await itemRepo.DeleteItemAsync(id, token);
        return (true);
    }
}
