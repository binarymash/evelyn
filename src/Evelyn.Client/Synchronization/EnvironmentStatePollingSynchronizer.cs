namespace Evelyn.Client.Synchronization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Provider;
    using Repository;

    public class EnvironmentStatePollingSynchronizer : BackgroundService
    {
        private readonly IEnvironmentStateProvider _provider;
        private readonly IEnvironmentStateRepository _repo;
        private readonly EnvironmentStatePollingSynchronizerOptions _pollingOptions;
        private readonly EnvironmentOptions _environmentOptions;

        public EnvironmentStatePollingSynchronizer(
            IEnvironmentStateProvider provider,
            IEnvironmentStateRepository repo,
            IOptions<EnvironmentStatePollingSynchronizerOptions> pollingOptions,
            IOptions<EnvironmentOptions> environmentOptions)
        {
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
                var environmentState = _provider.Invoke(_environmentOptions.ProjectId, _environmentOptions.Environment);
                _repo.Set(environmentState);
            }
            catch (Exception)
            {
                // TODO: log
            }
        }
    }
}
