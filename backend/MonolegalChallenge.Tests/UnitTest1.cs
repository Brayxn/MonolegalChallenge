using MonolegalChallenge.Application;
using MonolegalChallenge.Domain;
using Moq;

namespace MonolegalChallenge.Tests;

public class FacturaServiceTests
{
    [Fact]
    public async Task ProcesarRecordatoriosAsync_ActualizaEstadosYEnviaEmails()
    {
        // Arrange
        var facturas = new List<Factura>
        {
            new Factura { Id = "1", ClienteId = "A", Estado = "primerrecordatorio" },
            new Factura { Id = "2", ClienteId = "B", Estado = "segundorecordatorio" },
            new Factura { Id = "3", ClienteId = "C", Estado = "otro" }
        };
        var clientes = new List<Cliente>
        {
            new Cliente { Id = "A", Email = "a@mail.com" },
            new Cliente { Id = "B", Email = "b@mail.com" },
            new Cliente { Id = "C", Email = "c@mail.com" }
        };

        var facturaRepo = new Mock<IFacturaRepository>();
        facturaRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(facturas);
        facturaRepo.Setup(r => r.UpdateAsync(It.IsAny<Factura>())).Returns(Task.CompletedTask);

        var clienteRepo = new Mock<IClienteRepository>();
        clienteRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((string id) => clientes.FirstOrDefault(c => c.Id == id));

        var recordatorioProcessor = new Mock<IRecordatorioProcessor>();

        var service = new FacturaService(facturaRepo.Object, clienteRepo.Object, recordatorioProcessor.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert: processor debe ser invocado solo para facturas en primer/segundo recordatorio
        recordatorioProcessor.Verify(rp => rp.ProcesarRecordatorioIndividualAsync("1"), Times.Once);
        recordatorioProcessor.Verify(rp => rp.ProcesarRecordatorioIndividualAsync("2"), Times.Once);
        recordatorioProcessor.Verify(rp => rp.ProcesarRecordatorioIndividualAsync("3"), Times.Never);
    }
}
