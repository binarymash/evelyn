namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InMemoryDatabase<T> : IDatabase<T>
    {
        private Dictionary<Guid, T> _items;

        public InMemoryDatabase()
        {
            _items = new Dictionary<Guid, T>();
        }

        public async Task<T> Get(Guid id)
        {
            T value;
            if (!_items.TryGetValue(id, out value))
            {
                throw new NotFoundException();
            }

            return await Task.FromResult(value);
        }

        public async Task<List<T>> Get()
        {
            return await Task.FromResult(_items.Values.ToList());
        }

        public async Task Add(Guid id, T item)
        {
            _items.Add(id, item);
            await Task.CompletedTask;
        }
    }
}