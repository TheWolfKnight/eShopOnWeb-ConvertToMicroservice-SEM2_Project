using Microsoft.AspNetCore.Mvc;
using Microservice.Catalog.Infrastructure.Interfaces;
using Microservice.Catalog.Common.Models;

namespace CatalogMicroservice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ICatalogItemRepository _repo;

    public ItemsController(ICatalogItemRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogItem>>> GetPage(
        // en ide til at lave pagination og filtrering, her fra starten, skal nok ændres ERH
        [FromQuery] int pageNo = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? brandId = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        if (pageNo < 1 || pageSize < 1) return BadRequest("pageNo og pageSize skal være >= 1.");

        var items = await _repo.GetItemPageAsync(pageNo, pageSize, brandId, typeId, ct);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CatalogItem>> GetById(int id, CancellationToken ct)
    {
        var item = await _repo.GetItemAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogItem>> Create([FromBody] CreateCatalogItem dto, CancellationToken ct)
    {
        if (dto is null) return BadRequest("Body mangler.");

        var created = await _repo.CreateItemAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CatalogItem model, CancellationToken ct)
    {
        if (model is null || model.Id != id)
            return BadRequest("Id i route og body skal matche.");

        var existing = await _repo.GetItemAsync(id, ct);
        if (existing is null) return NotFound();

        await _repo.UpdateItemAsync(model, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var existing = await _repo.GetItemAsync(id, ct);
        if (existing is null) return NotFound();

        await _repo.DeleteItemAsync(id, ct);
        return NoContent();
    }
}
