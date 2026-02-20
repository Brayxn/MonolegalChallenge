using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application
{
    public interface ICorreoHistorialService
    {
        Task RegistrarEnvioAsync(string destinatario, string asunto, string facturaId, string cuerpo);
        Task<IEnumerable<CorreoHistorial>> ObtenerHistorialAsync();
        Task BorrarCorreoAsync(string id);
    }
}
