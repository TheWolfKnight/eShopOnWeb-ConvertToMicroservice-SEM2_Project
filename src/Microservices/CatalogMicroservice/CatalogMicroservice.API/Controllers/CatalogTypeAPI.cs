using Microsoft.AspNetCore.Mvc;
using CatalogMicroservice.Infrastructure.Interfaces;
using CatalogMicroservice.Common.Models;

namespace CatalogMicroservice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TypesController : ControllerBase
{
    private readonly ICatalogTypeRepository _repository;

    public TypesController(ICatalogTypeRepository repository) => _repository = repository;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogType>>> GetAll(CancellationToken ct)
    {
        var types = await _repository.GetCatalogTypesAsync(ct);
        return Ok(types);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CatalogType>> GetById(int id, CancellationToken ct)
    {
        var type = await _repository.GetCatalogTypeAsync(id, ct);
        return type is null ? NotFound() : Ok(type);
    }
}
