using Microsoft.Extensions.Hosting;
using Services.Services.Interfaces;
using Services.Utiltities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services1
{
    public class LMBBackgroundHost : IHostedService
    {
        private readonly PeriodicTimer _syncTimer = new PeriodicTimer(TimeSpan.FromDays(1));
        private readonly PeriodicTimer _antiSleepTimer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        private readonly IAnalysisService _analysisService;
        private readonly IPingService _pingService;
        public LMBBackgroundHost(IAnalysisService analysisService, IPingService pingService)
        {
            _analysisService = analysisService;
            _pingService = pingService;
        }

        private async Task DoSyncAsync(CancellationToken stoppingToken)
        {
            while (await _syncTimer.WaitForNextTickAsync(stoppingToken))
            {
                await _analysisService.DeleteOldDataAsync();
                await _analysisService.FlushAsync();
                await _analysisService.InitAsync();
            }
        }

        private async Task AntiSleepAsync(CancellationToken stoppingToken)
        {
            while (await _antiSleepTimer.WaitForNextTickAsync(stoppingToken))
            {
                await _pingService.PingAsync();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var endOfToday = DateTime.UtcNow.EndOfDate().AddHours(-2);
            var difference = (endOfToday - now).TotalMilliseconds;
            await _analysisService.InitAsync();
            Task.Run(async () =>
            {
                Console.WriteLine("Sync will start after: {0}", difference);
                await Task.Delay(Convert.ToInt32(difference));
                await DoSyncAsync(cancellationToken);
            });

            Task.Run(async () =>
            {
                await AntiSleepAsync(cancellationToken);
            });
           
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _syncTimer.Dispose();
            _antiSleepTimer.Dispose();
        }
    }
}
