using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Catalog.Item;

public record class ItemEntity
{
    public required int Id               { get; set; }
    public required string Name          { get; set; }
    public required string Description   { get; set; }
    public required decimal Price        { get; set; }
    public required string PictureUri    { get; set; }
    public required int CatalogTypeId    { get; set; }
    public object? CatalogType           { get; set; }
    public required int CatalogBrandId   { get; set; }
    public object? CatalogBrand          { get; set; }
}
