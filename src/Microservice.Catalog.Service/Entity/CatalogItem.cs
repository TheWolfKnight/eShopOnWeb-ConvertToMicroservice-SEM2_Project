using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Catalog.Service.Entity;

public record class CatalogItem
{
    public required int     Id           { get; set; }
    public required string  Name         { get; set; }
    public required string  Description  { get; set; }
    public required decimal Price        { get; set; }
    public required string  PictureUri   { get; set; }
    public required int     TypeId       { get; set; }
    public required int     BrandId      { get; set; }
}

public record class CatalogItemDTO
{
    public int     Id           { get; set; }
    public string  Name         { get; set; }
    public string  Description  { get; set; }
    public decimal Price        { get; set; }
    public string  PictureUri   { get; set; }
    public int     TypeId       { get; set; }
    public int     BrandId      { get; set; }
    public string  TypeName     { get; set; }
    public string  BrandName    { get; set; }
    public bool    IsValidMatch { get; set; } = true;

    public CatalogItemDTO(CatalogItem item, CatalogBrand brand, CatalogType type) 
    {
        if ((item.BrandId != brand.Id) || (item.TypeId != type.Id))
        {
            this.IsValidMatch = false;
        }
        
        this.Id           = item.Id;
        this.Name         = item.Name;
        this.Description  = item.Description;
        this.Price        = item.Price;
        this.PictureUri   = item.PictureUri;
        this.TypeId       = item.TypeId;
        this.BrandId      = item.BrandId;
        this.TypeName     = type.Type;
        this.BrandName    = brand.Brand;
    }
}
