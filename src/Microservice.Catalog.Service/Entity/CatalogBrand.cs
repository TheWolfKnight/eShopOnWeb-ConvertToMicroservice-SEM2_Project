using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Catalog.Service.Entity;

public record class CatalogBrand
{
    public required int    Id     { get; set; }
    public required string Brand  { get; set; }
}
