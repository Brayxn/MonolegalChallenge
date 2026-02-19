using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MonolegalChallenge.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly IMongoDatabase _db;
    public HealthController(IMongoDatabase db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Intenta listar colecciones para verificar conexión
            await _db.ListCollectionsAsync();
            return Ok(new { status = "ok" });
        }
        catch
        {
            return StatusCode(503, new { status = "error" });
        }
    }
}
