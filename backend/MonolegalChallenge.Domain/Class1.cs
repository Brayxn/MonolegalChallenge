

namespace MonolegalChallenge.Domain;

/// <summary>
/// Representa un cliente del sistema.
/// </summary>
public class Cliente
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Representa una factura asociada a un cliente.
/// </summary>
public class Factura
{
    public string Id { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Estado { get; set; } = string.Empty; // "primerrecordatorio", "segundorecordatorio", "desactivado", "activo"
    /// <summary>
    /// Fecha límite para desactivar la factura tras el segundo recordatorio.
    /// </summary>
    public DateTime? FechaLimiteDesactivacion { get; set; }
    /// <summary>
    /// Fecha en que se envió el primer recordatorio (UTC).
    /// </summary>
    public DateTime? FechaEnvioPrimerRecordatorio { get; set; }

    /// <summary>
    /// Fecha en que se envió el segundo recordatorio (UTC).
    /// </summary>
    public DateTime? FechaEnvioSegundoRecordatorio { get; set; }

    /// <summary>
    /// Fecha en que la factura fue marcada como pagada (UTC).
    /// </summary>
    public DateTime? FechaPago { get; set; }
}

/// <summary>
/// Contrato para el repositorio de clientes.
/// </summary>
public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(string id);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(string id);
}

/// <summary>
/// Contrato para el repositorio de facturas.
/// </summary>
public interface IFacturaRepository
{
    Task<Factura?> GetByIdAsync(string id);
    Task<IEnumerable<Factura>> GetByClienteIdAsync(string clienteId);
    Task<IEnumerable<Factura>> GetAllAsync();
    Task AddAsync(Factura factura);
    Task UpdateAsync(Factura factura);
    Task DeleteAsync(string id);
}

/// <summary>
/// Contrato para el servicio de notificación por email.
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
