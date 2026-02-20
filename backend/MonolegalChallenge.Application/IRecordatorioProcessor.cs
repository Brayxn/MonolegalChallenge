using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application
{
    public interface IRecordatorioProcessor
    {
        Task ProcesarRecordatorioIndividualAsync(string facturaId);
    }
}
