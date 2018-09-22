namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InMemoryProjectionStore<TValue> : IProjectionStore<TValue>
    {
        private readonly Dictionary<string, TValue> _items;

        public InMemoryProjectionStore()
        {
            _items = new Dictionary<string, TValue>();
        }

        public async Task<TValue> Get(string key)
        {
            if (!_items.TryGetValue(key, out var value))
            {
                throw new NotFoundException();
            }

            return await Task.FromResult(value);
        }

        public async Task AddOrUpdate(string key, TValue value)
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

        public async Task Delete(string key)
        {
            _items.Remove(key);
            await Task.CompletedTask;
        }
    }
}