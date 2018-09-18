namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System.Threading.Tasks;

    public interface IProjectionStore<TKey, TValue>
    {
        Task<TValue> Get(TKey id);

        Task AddOrUpdate(TKey key, TValue aggregate);

        Task Delete(TKey key);
    }
}
