namespace Evelyn.Core.ReadModel.Projections
{
    using System.Threading.Tasks;

    public interface IProjectionStore<TValue>
    {
        Task Create(string key, TValue aggregate);

        Task<TValue> Get(string key);

        Task Update(string key, TValue aggregate);

        Task Delete(string key);
    }
}
