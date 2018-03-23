namespace Evelyn.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEnvironmentStateRepository
    {
        Task Set(EnvironmentState environmentState);

        Task<bool> Get(string environmentKey);
    }
}
