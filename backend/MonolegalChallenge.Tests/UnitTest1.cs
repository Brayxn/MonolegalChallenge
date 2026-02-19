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

        var emailService = new Mock<IEmailService>();
        emailService.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var service = new FacturaService(facturaRepo.Object, clienteRepo.Object, emailService.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert
        facturaRepo.Verify(r => r.UpdateAsync(It.Is<Factura>(f => f.Id == "1" && f.Estado == "segundorecordatorio")), Times.Once);
        facturaRepo.Verify(r => r.UpdateAsync(It.Is<Factura>(f => f.Id == "2" && f.Estado == "desactivado")), Times.Once);
        emailService.Verify(e => e.SendEmailAsync("a@mail.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        emailService.Verify(e => e.SendEmailAsync("b@mail.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        // No se debe actualizar ni enviar email para facturas en otro estado
        facturaRepo.Verify(r => r.UpdateAsync(It.Is<Factura>(f => f.Id == "3")), Times.Never);
        emailService.Verify(e => e.SendEmailAsync("c@mail.com", It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
