using System.Net;
using System.Net.Mail;
using MonolegalChallenge.Domain;
using MongoDB.Driver;

namespace MonolegalChallenge.Infrastructure;

public class ClienteRepository : IClienteRepository
{
	private readonly IMongoCollection<Cliente> _clientes;
	public ClienteRepository(IMongoDatabase database)
	{
		_clientes = database.GetCollection<Cliente>("clientes");
	}
	public async Task<Cliente?> GetByIdAsync(string id) =>
		await _clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
	public async Task<IEnumerable<Cliente>> GetAllAsync() =>
		await _clientes.Find(_ => true).ToListAsync();
	public async Task AddAsync(Cliente cliente) =>
		await _clientes.InsertOneAsync(cliente);
	public async Task UpdateAsync(Cliente cliente) =>
		await _clientes.ReplaceOneAsync(c => c.Id == cliente.Id, cliente);
	public async Task DeleteAsync(string id) =>
		await _clientes.DeleteOneAsync(c => c.Id == id);
}

public class FacturaRepository : IFacturaRepository
{
	private readonly IMongoCollection<Factura> _facturas;
	public FacturaRepository(IMongoDatabase database)
	{
		_facturas = database.GetCollection<Factura>("facturas");
	}
	public async Task<Factura?> GetByIdAsync(string id) =>
		await _facturas.Find(f => f.Id == id).FirstOrDefaultAsync();
	public async Task<IEnumerable<Factura>> GetByClienteIdAsync(string clienteId) =>
		await _facturas.Find(f => f.ClienteId == clienteId).ToListAsync();
	public async Task<IEnumerable<Factura>> GetAllAsync() =>
		await _facturas.Find(_ => true).ToListAsync();
	public async Task AddAsync(Factura factura) =>
		await _facturas.InsertOneAsync(factura);
	public async Task UpdateAsync(Factura factura) =>
		await _facturas.ReplaceOneAsync(f => f.Id == factura.Id, factura);
	public async Task DeleteAsync(string id) =>
		await _facturas.DeleteOneAsync(f => f.Id == id);
}

public class EmailService : IEmailService
{
	private readonly SmtpSettings _smtpSettings;

	public EmailService(SmtpSettings smtpSettings)
	{
		_smtpSettings = smtpSettings;
	}

	public async Task SendEmailAsync(string to, string subject, string body)
	{
		using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
		{
			Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Pass),
			EnableSsl = true
		};
		var mail = new MailMessage(_smtpSettings.From, to, subject, body);
		await client.SendMailAsync(mail);
	}
}
