namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryDatabase<T> : IDatabase<T>
    {
        private Dictionary<Guid, T> _items;

        public InMemoryDatabase()
        {
            _items = new Dictionary<Guid, T>();
        }

        public T Get(Guid id)
        {
            T value;
            if (!_items.TryGetValue(id, out value))
            {
                throw new NotFoundException();
            }

            return value;
        }

        public List<T> Get()
        {
            return _items.Values.ToList();
        }

        public void Add(Guid id, T item)
        {
            _items.Add(id, item);
        }
    }
}