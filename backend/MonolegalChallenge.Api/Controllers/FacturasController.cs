
using Microsoft.AspNetCore.Mvc;
using MonolegalChallenge.Application;
using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
            private readonly FacturaService _facturaService;
            private readonly int _tiempoSegundoRecordatorioMin;

            public FacturasController(FacturaService facturaService, IConfiguration config)
            {
                _facturaService = facturaService;
                _tiempoSegundoRecordatorioMin = config.GetValue<int?>("TiempoSegundoRecordatorioMin") ?? 40;
            }

        [HttpPost("procesar-recordatorio/{id}")]
        public async Task<IActionResult> ProcesarRecordatorioIndividual(string id)
        {
            await _facturaService.ProcesarRecordatorioIndividualAsync(id);
            return Ok(new { message = $"Recordatorio procesado para factura {id}" });
        }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFactura(string id, [FromBody] Factura factura)
    {
        if (id != factura.Id)
            return BadRequest("El id de la URL no coincide con el id del body");
        await _facturaService.ActualizarFacturaAsync(factura);
        return Ok(new { message = "Factura actualizada" });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var facturas = await _facturaService.ObtenerTodasFacturasAsync();
        // Obtener todos los clientes para hacer join en memoria
        var clientes = await _facturaService.ObtenerTodosClientesAsync();
        var result = facturas.Select(f => {
            var cliente = clientes.FirstOrDefault(c => c.Id == f.ClienteId);
            DateTime? fechaProgramadaSegundo = null;
            if (f.FechaEnvioPrimerRecordatorio != null && f.FechaEnvioSegundoRecordatorio == null)
            {
                fechaProgramadaSegundo = f.FechaEnvioPrimerRecordatorio.Value
                   .AddMinutes(_tiempoSegundoRecordatorioMin)
                   .ToUniversalTime();
            }
            return new {
                id = f.Id,
                clienteId = f.ClienteId,
                clienteNombre = cliente != null ? cliente.Nombre : "",
                monto = f.Monto,
                emision = f.FechaEmision.ToString("yyyy-MM-dd"),
                estado = f.Estado,
                fechaLimiteDesactivacion = f.FechaLimiteDesactivacion,
                fechaProgramadaSegundo = fechaProgramadaSegundo?.ToString("o"),
                fechaPago = f.FechaPago,
                descripcion = "",
                limite = f.FechaLimiteDesactivacion,
            };
        });
        return Ok(result);
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<Factura>>> GetByCliente(string clienteId)
    {
        var facturas = await _facturaService.ObtenerFacturasPorClienteAsync(clienteId);
        return Ok(facturas);
    }

    [HttpPost("procesar-recordatorios")]
    public async Task<IActionResult> ProcesarRecordatorios()
    {
        await _facturaService.ProcesarRecordatoriosAsync();
        return Ok(new { message = "Recordatorios procesados" });
    }

    [HttpPost("{id}/poner-al-dia")]
    public async Task<IActionResult> PonerAlDia(string id)
    {
        await _facturaService.MarcarFacturaActivaAsync(id);
        return Ok(new { message = $"Factura {id} marcada como al día" });
    }


    }
}
