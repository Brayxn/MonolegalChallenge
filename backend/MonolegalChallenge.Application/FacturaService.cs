using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application;

/// <summary>
/// Servicio de aplicación para operaciones de facturas.
/// </summary>
public class FacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;
    private readonly CorreoHistorialService _correoHistorialService;
    private readonly TimeSpan _tiempoEsperaDesactivacion;

    public FacturaService(
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository,
        IEmailService emailService,
        CorreoHistorialService correoHistorialService,
        TimeSpan? tiempoEsperaDesactivacion = null)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
        _emailService = emailService;
        _correoHistorialService = correoHistorialService;
        _tiempoEsperaDesactivacion = tiempoEsperaDesactivacion ?? TimeSpan.FromHours(24);
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosClientesAsync()
    {
        return await _clienteRepository.GetAllAsync();
    }

    public async Task ProcesarRecordatorioIndividualAsync(string facturaId)
    {
        var factura = await _facturaRepository.GetByIdAsync(facturaId);
        if (factura == null) return;
        var cliente = await _clienteRepository.GetByIdAsync(factura.ClienteId);
        if (cliente == null) return;

        if (factura.Estado == "primerrecordatorio")
        {
            var body = EmailTemplateService.BuildBodyPrimerRecordatorio(cliente.Nombre, factura.Id, factura.Monto);
            await _emailService.SendEmailAsync(cliente.Email, "Primer recordatorio de pago", body);
            await _correoHistorialService.RegistrarEnvioAsync(cliente.Email, "Primer recordatorio de pago", factura.Id, body);
            factura.Estado = "segundorecordatorio";
            factura.FechaLimiteDesactivacion = null;
            await _facturaRepository.UpdateAsync(factura);
        }
        else if (factura.Estado == "segundorecordatorio")
        {
            if (factura.FechaLimiteDesactivacion == null)
            {
                var ahora = DateTime.UtcNow;
                var fechaLimite = ahora.Add(_tiempoEsperaDesactivacion);
                var body = EmailTemplateService.BuildBodySegundoRecordatorio(
                    cliente.Nombre,
                    factura.Id,
                    factura.Monto,
                    _tiempoEsperaDesactivacion,
                    fechaLimite.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"));
                await _emailService.SendEmailAsync(cliente.Email, "Segundo recordatorio de pago", body);
                await _correoHistorialService.RegistrarEnvioAsync(cliente.Email, "Segundo recordatorio de pago", factura.Id, body);
                factura.FechaLimiteDesactivacion = fechaLimite;
                await _facturaRepository.UpdateAsync(factura);
            }
            // La desactivación automática se realiza solo por el servicio en segundo plano
        }
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
