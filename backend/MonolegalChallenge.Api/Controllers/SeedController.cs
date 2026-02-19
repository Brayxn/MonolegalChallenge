using Microsoft.AspNetCore.Mvc;
using MonolegalChallenge.Domain;
using MongoDB.Driver;

namespace MonolegalChallenge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly IMongoDatabase _db;
    public SeedController(IMongoDatabase db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        var clientes = new[]
        {
            new Cliente { Id = "1", Nombre = "Cliente Uno", Email = "cliente1@mail.com" },
            new Cliente { Id = "2", Nombre = "Cliente Dos", Email = "cliente2@mail.com" },
            new Cliente { Id = "3", Nombre = "Cliente Tres", Email = "cliente3@mail.com" }
        };
        var facturas = new[]
        {
            new Factura { Id = "F1", ClienteId = "1", Monto = 1000, FechaEmision = DateTime.UtcNow.AddDays(-20), Estado = "primerrecordatorio" },
            new Factura { Id = "F2", ClienteId = "2", Monto = 2000, FechaEmision = DateTime.UtcNow.AddDays(-30), Estado = "segundorecordatorio" },
            new Factura { Id = "F3", ClienteId = "3", Monto = 3000, FechaEmision = DateTime.UtcNow.AddDays(-40), Estado = "primerrecordatorio" }
        };
        await _db.GetCollection<Cliente>("clientes").DeleteManyAsync(_ => true);
        await _db.GetCollection<Factura>("facturas").DeleteManyAsync(_ => true);
        await _db.GetCollection<Cliente>("clientes").InsertManyAsync(clientes);
        await _db.GetCollection<Factura>("facturas").InsertManyAsync(facturas);
        return Ok(new { message = "Datos de prueba insertados" });
    }
}
