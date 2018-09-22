namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System.Threading.Tasks;

    public interface IProjectionStore<TValue>
    {
        Task<TValue> Get(string id);

        Task AddOrUpdate(string key, TValue aggregate);

        Task Delete(string key);
    }
}
