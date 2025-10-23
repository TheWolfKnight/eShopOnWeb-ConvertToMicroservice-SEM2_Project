using Microsoft.AspNetCore.Mvc;
using Microservice.Catalog.Infrastructure.Interfaces;
using Microservice.Catalog.Common.Models;

namespace CatalogMicroservice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly ICatalogBrandRepository _repo;

    public BrandsController(ICatalogBrandRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogBrand>>> GetAll(CancellationToken ct)
    {
        var brands = await _repo.GetBrandsAsync(ct);
        return Ok(brands);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CatalogBrand>> GetById(int id, CancellationToken ct)
    {
        var brand = await _repo.GetBrandByIdAsync(id, ct);
        return brand is null ? NotFound() : Ok(brand);
    }
}
