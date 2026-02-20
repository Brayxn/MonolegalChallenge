using Microsoft.AspNetCore.Mvc;
using MonolegalChallenge.Application;
using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CorreosController : ControllerBase
    {
        private readonly ICorreoHistorialService _service;
        public CorreosController(ICorreoHistorialService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CorreoHistorial>>> GetAll()
        {
            var historial = await _service.ObtenerHistorialAsync();
            return Ok(historial);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.BorrarCorreoAsync(id);
            return NoContent();
        }
    }
}
