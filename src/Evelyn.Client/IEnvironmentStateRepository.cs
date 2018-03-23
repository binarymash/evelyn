namespace Evelyn.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEnvironmentStateRepository
    {
        Task Set(EnvironmentState environmentState, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> Get(string environmentKey, CancellationToken cancellationToken = default(CancellationToken));
    }
}
