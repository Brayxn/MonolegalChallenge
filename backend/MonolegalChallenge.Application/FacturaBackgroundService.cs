
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MonolegalChallenge.Application
{
    public class FacturaBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _intervalo;

        public FacturaBackgroundService(IServiceProvider serviceProvider, TimeSpan? intervalo = null)
        {
            _serviceProvider = serviceProvider;
            _intervalo = intervalo ?? TimeSpan.FromMinutes(1); // Por defecto 1 minuto
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
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
                }
                await Task.Delay(_intervalo, stoppingToken);
            }
        }
    }
}
