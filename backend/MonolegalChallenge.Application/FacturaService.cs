using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application;

/// <summary>
/// Servicio de aplicación para operaciones de facturas.
/// </summary>
public class FacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IRecordatorioProcessor _recordatorioProcessor;

    public FacturaService(
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository,
        IRecordatorioProcessor recordatorioProcessor)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
        _recordatorioProcessor = recordatorioProcessor;
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosClientesAsync()
    {
        return await _clienteRepository.GetAllAsync();
    }

    public async Task ProcesarRecordatorioIndividualAsync(string facturaId)
    {
        await _recordatorioProcessor.ProcesarRecordatorioIndividualAsync(facturaId);
    }

    public async Task ActualizarFacturaAsync(Factura factura)
    {
        await _facturaRepository.UpdateAsync(factura);
    }

    /// <summary>
    /// Procesa recordatorios para todas las facturas en estado pendiente (primer o segundo recordatorio).
    /// </summary>
    public async Task ProcesarRecordatoriosAsync()
    {
        var facturas = await _facturaRepository.GetAllAsync();
        foreach (var factura in facturas)
        {
            if (factura.Estado == "primerrecordatorio" || factura.Estado == "segundorecordatorio")
            {
                await ProcesarRecordatorioIndividualAsync(factura.Id);
            }
        }
    }

    public async Task<IEnumerable<Factura>> ObtenerFacturasPorClienteAsync(string clienteId)
    {
        return await _facturaRepository.GetByClienteIdAsync(clienteId);
    }

    public async Task<IEnumerable<Factura>> ObtenerTodasFacturasAsync()
    {
        return await _facturaRepository.GetAllAsync();
    }
}
