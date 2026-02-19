using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application;

/// <summary>
/// Servicio de aplicación para operaciones de clientes.
/// </summary>
public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
    {
        return await _clienteRepository.GetAllAsync();
    }
}
