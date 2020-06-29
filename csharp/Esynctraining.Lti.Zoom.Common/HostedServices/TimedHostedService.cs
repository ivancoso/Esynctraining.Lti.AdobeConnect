using System;
using System.Threading;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.Extensions.Hosting;

namespace Esynctraining.Lti.Zoom.Common.HostedServices
{
    public abstract class TimedHostedService : IHostedService, IDisposable
    {
        protected ILogger Logger { get; }
        private Timer _timer;
        private readonly dynamic _settings;

        protected TimedHostedService(ILogger logger, ApplicationSettingsProvider settings)
        {
            Logger = logger;
            _settings = settings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.Info("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(double.Parse((string)_settings.LicenseUsersCacheDuration)));

            return Task.CompletedTask;
        }

        protected abstract void DoWork(object state);

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
