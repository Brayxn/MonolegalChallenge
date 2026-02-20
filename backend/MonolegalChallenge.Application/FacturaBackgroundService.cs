
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MonolegalChallenge.Domain;

namespace MonolegalChallenge.Application
{
    public class FacturaBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _intervalo;
        private readonly TimeSpan _tiempoEntreRecordatorios;

        public FacturaBackgroundService(IServiceScopeFactory scopeFactory, TimeSpan? intervalo = null, TimeSpan? tiempoEntreRecordatorios = null)
        {
            _scopeFactory = scopeFactory;
            _intervalo = intervalo ?? TimeSpan.FromMinutes(1); // Por defecto 1 minuto
            _tiempoEntreRecordatorios = tiempoEntreRecordatorios ?? TimeSpan.FromMinutes(1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var autoDesactivacionService = scope.ServiceProvider.GetRequiredService<FacturaAutoDesactivacionService>();
                    try
                    {
                        await autoDesactivacionService.ProcesarDesactivacionesAsync();
                    }
                    catch (Exception ex)
                    {
                        // Loguear error si se desea
                        Console.WriteLine($"[ERROR] FacturaBackgroundService: {ex.Message}");
                    }
                    // Procesar envío automático del segundo recordatorio cuando corresponda
                    try
                    {
                        var facturaRepo = scope.ServiceProvider.GetRequiredService<IFacturaRepository>();
                        var recordatorioProcessor = scope.ServiceProvider.GetRequiredService<IRecordatorioProcessor>();
                        var facturas = await facturaRepo.GetAllAsync();
                        var ahora = DateTime.UtcNow;
                        foreach (var factura in facturas)
                        {
                            if (factura.Estado == "segundorecordatorio" &&
                                factura.FechaEnvioPrimerRecordatorio != null &&
                                factura.FechaEnvioSegundoRecordatorio == null)
                            {
                                var envioPrevisto = factura.FechaEnvioPrimerRecordatorio.Value.Add(_tiempoEntreRecordatorios);
                                if (envioPrevisto <= ahora)
                                {
                                    try
                                    {
                                        await recordatorioProcessor.ProcesarRecordatorioIndividualAsync(factura.Id);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Envío automático segundo recordatorio factura {factura.Id}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] FacturaBackgroundService (segundos): {ex.Message}");
                    }
                }
                await Task.Delay(_intervalo, stoppingToken);
            }
        }
    }
}
