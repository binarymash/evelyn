namespace Evelyn.Client.Synchronization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Provider;
    using Repository;

    public class EnvironmentStateSynchronizer : BackgroundService
    {
        private readonly IEnvironmentStateProvider _provider;
        private readonly IEnvironmentStateRepository _repo;
        private readonly EnvironmentStateSynchronizerOptions _options;

        public EnvironmentStateSynchronizer(IEnvironmentStateProvider provider, IEnvironmentStateRepository repo, IOptions<EnvironmentStateSynchronizerOptions> options)
        {
            _provider = provider;
            _repo = repo;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                SynchronizeEnvironmentStates();
                await Task.Delay(_options.SynchronizationPeriod, stoppingToken);
            }
        }

        private void SynchronizeEnvironmentStates()
        {
            try
            {
                var environmentState = _provider.Invoke(_options.ProjectId, _options.Environment);
                _repo.Set(environmentState);
            }
            catch (Exception)
            {
                // TODO: log
            }
        }
    }
}
