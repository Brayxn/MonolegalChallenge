using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application
{
    public class CorreoHistorialService : ICorreoHistorialService
    {
        private readonly ICorreoHistorialRepository _repo;
        public CorreoHistorialService(ICorreoHistorialRepository repo)
        {
            _repo = repo;
        }
        public async Task RegistrarEnvioAsync(string destinatario, string asunto, string facturaId, string cuerpo)
        {
            var correo = new CorreoHistorial
            {
                Destinatario = destinatario,
                Asunto = asunto,
                FacturaId = facturaId,
                Fecha = DateTime.UtcNow,
                Cuerpo = cuerpo ?? string.Empty
            };
            await _repo.AddAsync(correo);
        }
        public async Task<IEnumerable<CorreoHistorial>> ObtenerHistorialAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task BorrarCorreoAsync(string id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
