using System;

namespace MonolegalChallenge.Domain
{
    public class CorreoHistorial
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Destinatario { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string FacturaId { get; set; } = string.Empty;
            public string Cuerpo { get; set; } = string.Empty; // Se serializa y expone en el endpoint
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }

    public interface ICorreoHistorialRepository
    {
        Task AddAsync(CorreoHistorial correo);
        Task<IEnumerable<CorreoHistorial>> GetAllAsync();
        Task DeleteAsync(string id);
    }
}
