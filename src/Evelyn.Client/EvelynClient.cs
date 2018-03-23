namespace Evelyn.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class EvelynClient : IEvelynClient
    {
        private readonly IEnvironmentStateRepository _repo;

        public EvelynClient(IEnvironmentStateRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> GetToggleState(string toggleKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await _repo.Get(toggleKey, cancellationToken);
            }
            catch (Exception)
            {
                // TODO: logging
            }

            return default(bool);
        }
    }
}
