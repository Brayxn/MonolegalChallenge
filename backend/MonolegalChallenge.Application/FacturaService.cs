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
        private readonly IEmailService _emailService;
        private readonly ICorreoHistorialService _correoHistorialService;
        private readonly IEmailTemplateService _emailTemplateService;

    public FacturaService(
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository,
        IRecordatorioProcessor recordatorioProcessor,
        IEmailService emailService,
        ICorreoHistorialService correoHistorialService,
        IEmailTemplateService emailTemplateService)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
        _recordatorioProcessor = recordatorioProcessor;
        _emailService = emailService;
        _correoHistorialService = correoHistorialService;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosClientesAsync()
    {
        return await _clienteRepository.GetAllAsync();
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
                await _recordatorioProcessor.ProcesarRecordatorioIndividualAsync(factura.Id);
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

    public async Task MarcarFacturaActivaAsync(string facturaId)
    {
        var factura = await _facturaRepository.GetByIdAsync(facturaId);
        if (factura == null) return;
        var cliente = await _clienteRepository.GetByIdAsync(factura.ClienteId);
        if (cliente == null) return;

        factura.Estado = "activo";
        factura.FechaLimiteDesactivacion = null;
        factura.FechaPago = DateTime.UtcNow;
        await _facturaRepository.UpdateAsync(factura);

        var body = _emailTemplateService.BuildBodyPagoRecibido(cliente.Nombre, factura.Id, factura.Monto);
        await _emailService.SendEmailAsync(cliente.Email, "Pago recibido", body);
        await _correoHistorialService.RegistrarEnvioAsync(cliente.Email, "Pago recibido", factura.Id, body);
    }

    // Permitir procesar desde 'activo' a 'primerrecordatorio' y enviar primer recordatorio
    public async Task ProcesarRecordatorioIndividualAsync(string facturaId)
    {
        var factura = await _facturaRepository.GetByIdAsync(facturaId);
        if (factura == null) return;
        var cliente = await _clienteRepository.GetByIdAsync(factura.ClienteId);
        if (cliente == null) return;

        if (factura.Estado == "activo")
        {
            // Cambiar a primerrecordatorio y enviar correo
            factura.Estado = "primerrecordatorio";
            factura.FechaEnvioPrimerRecordatorio = DateTime.UtcNow;
            var body = _emailTemplateService.BuildBodyPrimerRecordatorio(cliente.Nombre, factura.Id, factura.Monto);
            await _emailService.SendEmailAsync(cliente.Email, "Primer recordatorio de pago", body);
            await _correoHistorialService.RegistrarEnvioAsync(cliente.Email, "Primer recordatorio de pago", factura.Id, body);
            await _facturaRepository.UpdateAsync(factura);
            return;
        }

        // Si ya está en primerrecordatorio o segundorecordatorio, delegar al processor
        await _recordatorioProcessor.ProcesarRecordatorioIndividualAsync(facturaId);
    }
}
