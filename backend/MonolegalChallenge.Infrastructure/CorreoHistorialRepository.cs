using MonolegalChallenge.Domain;
using MongoDB.Driver;

namespace MonolegalChallenge.Infrastructure
{
    public class CorreoHistorialRepository : ICorreoHistorialRepository
    {
        private readonly IMongoCollection<CorreoHistorial> _collection;

        public CorreoHistorialRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<CorreoHistorial>("correo_historial");
        }

        public async Task AddAsync(CorreoHistorial correo)
        {
            await _collection.InsertOneAsync(correo);
        }

        public async Task<IEnumerable<CorreoHistorial>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(c => c.Id == id);
        }
    }
}
