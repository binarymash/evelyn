namespace Evelyn.Client.Synchronization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Provider;
    using Repository;

    public class EnvironmentStatePollingSynchronizer : BackgroundService
    {
        private readonly ILogger<EnvironmentStatePollingSynchronizer> _logger;
        private readonly IEnvironmentStateProvider _provider;
        private readonly IEnvironmentStateRepository _repo;
        private readonly EnvironmentStatePollingSynchronizerOptions _pollingOptions;
        private readonly EnvironmentOptions _environmentOptions;

        public EnvironmentStatePollingSynchronizer(
            ILogger<EnvironmentStatePollingSynchronizer> logger,
            IEnvironmentStateProvider provider,
            IEnvironmentStateRepository repo,
            IOptions<EnvironmentStatePollingSynchronizerOptions> pollingOptions,
            IOptions<EnvironmentOptions> environmentOptions)
        {
            _logger = logger;
            _provider = provider;
            _repo = repo;
            _pollingOptions = pollingOptions.Value;
            _environmentOptions = environmentOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                SynchronizeEnvironmentStates();
                await Task.Delay(_pollingOptions.PollingPeriod, stoppingToken);
            }
        }

        private void SynchronizeEnvironmentStates()
        {
            try
            {
                _logger.LogTrace("Synchronizing {@projectId} {@environmentKey}", _environmentOptions.ProjectId, _environmentOptions.Environment);
                var environmentState = _provider.Invoke(_environmentOptions.ProjectId, _environmentOptions.Environment);
                _repo.Set(environmentState);
                _logger.LogTrace("Synchronized {@projectId} {@environmentKey}", _environmentOptions.ProjectId, _environmentOptions.Environment);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to synchronize {@projectId} {@environmentKey}. Toggle states might be stale.", _environmentOptions.ProjectId, _environmentOptions.Environment);
            }
        }
    }
}
