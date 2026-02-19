using Microsoft.AspNetCore.Mvc;
using MonolegalChallenge.Application;
using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAll()
    {
        var clientes = await _clienteService.ObtenerTodosAsync();
        return Ok(clientes);
    }
}
