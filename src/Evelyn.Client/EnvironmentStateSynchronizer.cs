namespace Evelyn.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class EnvironmentStateSynchronizer : BackgroundService
    {
        private readonly IEnvironmentStateProvider _provider;
        private readonly IEnvironmentStateRepository _repo;
        private readonly Guid _projectId;
        private readonly string _environmentKey;

        public EnvironmentStateSynchronizer(IEnvironmentStateProvider provider, IEnvironmentStateRepository repo)
        {
            _provider = provider;
            _repo = repo;
            _projectId = Guid.Parse("{222649E0-1E2D-4A1A-B986-3400CEC08B49}");
            _environmentKey = "development";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                SynchronizeEnvironmentStates();
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private void SynchronizeEnvironmentStates()
        {
            try
            {
                var environmentState = _provider.Invoke(_projectId, _environmentKey);
                _repo.Set(environmentState);
            }
            catch (Exception)
            {
                // TODO: log
            }
        }
    }
}
