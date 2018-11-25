namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Newtonsoft.Json;

    public class InMemoryProjectionStore<TValue> : IProjectionStore<TValue>
    {
        private readonly Dictionary<string, TValue> _items;
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        public InMemoryProjectionStore()
        {
            _items = new Dictionary<string, TValue>();
            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };
        }

        public async Task Create(string key, TValue value)
        {
            if (_items.ContainsKey(key))
            {
                throw new ProjectionAlreadyExistsException();
            }

            _items.Add(key, CopyOf(value));

            await Task.CompletedTask;
        }

        public async Task<TValue> Get(string key)
        {
            if (!_items.TryGetValue(key, out var value))
            {
                throw new ProjectionNotFoundException();
            }

            return await Task.FromResult(CopyOf(value));
        }

        public async Task Update(string key, TValue value)
        {
            if (!_items.ContainsKey(key))
            {
                throw new ProjectionNotFoundException();
            }

            _items[key] = CopyOf(value);

            await Task.CompletedTask;
        }

        public async Task Delete(string key)
        {
            _items.Remove(key);
            await Task.CompletedTask;
        }

        private TValue CopyOf(TValue original)
        {
            var serializedDto = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<TValue>(serializedDto, _deserializeWithPrivateSetters);
        }
    }
}