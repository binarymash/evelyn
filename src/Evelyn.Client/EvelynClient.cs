namespace Evelyn.Client
{
    using System.Threading.Tasks;

    public class EvelynClient : IEvelynClient
    {
        private readonly IEnvironmentStateRepository _repo;

        public EvelynClient(IEnvironmentStateRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> GetToggleState(string toggleKey)
        {
            return await _repo.Get(toggleKey);
        }
    }
}
