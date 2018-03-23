namespace Evelyn.Client
{
    using System.Threading.Tasks;

    public interface IEvelynClient
    {
        Task<bool> GetToggleState(string toggleKey);
    }
}
