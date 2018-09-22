namespace Evelyn.Core.ReadModel.Projections
{
    using System.Threading.Tasks;

    public interface IProjectionStore<TValue>
    {
        Task<TValue> Get(string id);

        Task AddOrUpdate(string key, TValue aggregate);

        Task Delete(string key);
    }
}
