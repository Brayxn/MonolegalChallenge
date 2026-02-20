namespace MonolegalChallenge.Application
{
    public interface IEmailTemplateService
    {
        string BuildBodyPrimerRecordatorio(string nombre, string facturaId, decimal monto);
        string BuildBodySegundoRecordatorio(string nombre, string facturaId, decimal monto, TimeSpan tiempoEspera, string? fechaLimiteDesactivacion = null);
        string BuildBodyDesactivado(string nombre, string facturaId, decimal monto);
    }
}
