namespace Evelyn.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEvelynClient
    {
        Task<bool> GetToggleState(string toggleKey, CancellationToken cancellationToken = default(CancellationToken));
    }
}
