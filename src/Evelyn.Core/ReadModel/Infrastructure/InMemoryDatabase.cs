namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InMemoryProjectionStore<TKey, TValue> : IProjectionStore<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _items;

        public InMemoryProjectionStore()
        {
            _items = new Dictionary<TKey, TValue>();
        }

        public async Task<TValue> Get(TKey key)
        {
            if (!_items.TryGetValue(key, out var value))
            {
                throw new NotFoundException();
            }

            return await Task.FromResult(value);
        }

        public async Task AddOrUpdate(TKey key, TValue value)
        {
            if (_items.ContainsKey(key))
            {
                _items[key] = value;
            }
            else
            {
                _items.Add(key, value);
            }

            await Task.CompletedTask;
        }

        public async Task Delete(TKey key)
        {
            _items.Remove(key);
            await Task.CompletedTask;
        }
    }
}