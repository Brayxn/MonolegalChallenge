using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application;

public class RecordatorioProcessor : IRecordatorioProcessor
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;
    private readonly ICorreoHistorialService _correoHistorialService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly TimeSpan _tiempoEsperaDesactivacion;

    public RecordatorioProcessor(
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository,
        IEmailService emailService,
        ICorreoHistorialService correoHistorialService,
        IEmailTemplateService emailTemplateService,
        TimeSpan tiempoEsperaDesactivacion)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
        _emailService = emailService;
        _correoHistorialService = correoHistorialService;
        _emailTemplateService = emailTemplateService;
        _tiempoEsperaDesactivacion = tiempoEsperaDesactivacion;
    }

    public async Task ProcesarRecordatorioIndividualAsync(string facturaId)
    {
        var factura = await _facturaRepository.GetByIdAsync(facturaId);
        if (factura == null) return;
        var cliente = await _clienteRepository.GetByIdAsync(factura.ClienteId);
        if (cliente == null) return;

        if (factura.Estado == "primerrecordatorio")
        {
            var ahora = DateTime.UtcNow;
            var body = _emailTemplateService.BuildBodyPrimerRecordatorio(cliente.Nombre, factura.Id, factura.Monto);
            await _emailService.SendEmailAsync(cliente.Email, "Primer recordatorio de pago", body);
            await _correoHistorialService.RegistrarEnvioAsync(cliente.Email, "Primer recordatorio de pago", factura.Id, body);
            factura.Estado = "segundorecordatorio";
            factura.FechaLimiteDesactivacion = null;
            factura.FechaEnvioPrimerRecordatorio = ahora;
            await _facturaRepository.UpdateAsync(factura);
        }
        else if (factura.Estado == "segundorecordatorio")
        {
            // Envío del segundo recordatorio: se hace sólo si no se ha enviado ya (FechaEnvioSegundoRecordatorio == null)
            if (factura.FechaEnvioSegundoRecordatorio == null)
            {
                var ahora = DateTime.UtcNow;
                var fechaLimite = ahora.Add(_tiempoEsperaDesactivacion);
                var body = _emailTemplateService.BuildBodySegundoRecordatorio(
                    cliente.Nombre,
                    factura.Id,
                    factura.Monto,
                    _tiempoEsperaDesactivacion,
                    fechaLimite.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"));
                await _emailService.SendEmailAsync(cliente.Email, "Segundo recordatorio de pago", body);
                await _correoHistorialService.RegistrarEnvioAsync(cliente.Email, "Segundo recordatorio de pago", factura.Id, body);
                factura.FechaEnvioSegundoRecordatorio = ahora;
                factura.FechaLimiteDesactivacion = fechaLimite;
                await _facturaRepository.UpdateAsync(factura);
            }
        }
    }
}
