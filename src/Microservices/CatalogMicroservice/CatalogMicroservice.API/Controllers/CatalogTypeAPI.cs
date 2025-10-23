using Microsoft.AspNetCore.Mvc;
using Microservice.Catalog.Infrastructure.Interfaces;
using Microservice.Catalog.Common.Models;

namespace CatalogMicroservice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TypesController : ControllerBase
{
    private readonly ICatalogTypeRepository _repo;

    public TypesController(ICatalogTypeRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogType>>> GetAll(CancellationToken ct)
    {
        var types = await _repo.GetCatalogTypesAsync(ct);
        return Ok(types);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CatalogType>> GetById(int id, CancellationToken ct)
    {
        var type = await _repo.GetCatalogTypeAsync(id, ct);
        return type is null ? NotFound() : Ok(type);
    }
}
