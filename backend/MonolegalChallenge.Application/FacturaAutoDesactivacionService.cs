using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application;


public class FacturaAutoDesactivacionService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;
    private readonly ICorreoHistorialService _correoHistorialService;
    private readonly IEmailTemplateService _emailTemplateService;

    public FacturaAutoDesactivacionService(
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository,
        IEmailService emailService,
        ICorreoHistorialService correoHistorialService,
        IEmailTemplateService emailTemplateService)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
        _emailService = emailService;
        _correoHistorialService = correoHistorialService;
        _emailTemplateService = emailTemplateService;
    }

    public async Task ProcesarDesactivacionesAsync()
    {
        var facturas = await _facturaRepository.GetAllAsync();
        var ahora = DateTime.UtcNow;
        foreach (var factura in facturas)
        {
            if (factura.Estado == "segundorecordatorio" &&
                factura.FechaLimiteDesactivacion != null &&
                factura.FechaLimiteDesactivacion <= ahora)
            {
                var cliente = await _clienteRepository.GetByIdAsync(factura.ClienteId);
                if (cliente == null) continue;
                var body = _emailTemplateService.BuildBodyDesactivado(cliente.Nombre, factura.Id, factura.Monto);
                await _emailService.SendEmailAsync(cliente.Email, "Factura desactivada", body);
                await _correoHistorialService.RegistrarEnvioAsync(cliente.Email, "Factura desactivada", factura.Id, body);
                factura.Estado = "desactivado";
                factura.FechaLimiteDesactivacion = null;
                await _facturaRepository.UpdateAsync(factura);
            }
        }
    }
}
