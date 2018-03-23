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

        public bool GetToggleState(string toggleKey)
        {
            return _repo.Get(toggleKey);
        }
    }
}
