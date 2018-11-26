namespace Evelyn.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;

    /// <summary>
    /// This is a naive implementation of an event store that really shouldn't be
    /// used for anything important!
    /// </summary>
    public class InMemoryEventStore : IInMemoryEventStore
    {
        private readonly IEventPublisher _publisher;
        private readonly ConcurrentDictionary<Guid, List<IEvent>> _events = new ConcurrentDictionary<Guid, List<IEvent>>();
        private readonly ConcurrentDictionary<long, IEvent> _eventStream = new ConcurrentDictionary<long, IEvent>();

        public InMemoryEventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var @event in events)
            {
                _eventStream.AddOrUpdate(_eventStream.Keys.Count, @event, (key, value) => throw new ConcurrencyException(@event.Id));

                _events.TryGetValue(@event.Id, out var list);
                if (list == null)
                {
                    list = new List<IEvent>();
                    _events.AddOrUpdate(@event.Id, list, (key, value) => throw new ConcurrencyException(@event.Id));
                }

                list.Add(@event);

                await _publisher.Publish(@event, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            _events.TryGetValue(aggregateId, out var events);
            return Task.FromResult(events?.Where(x => x.Version > fromVersion) ?? new List<IEvent>());
        }

        // TODO: make this a subscription
        public IEnumerable<InMemoryEventEnvelope> Get(long fromPosition)
        {
            var stream = _eventStream
                .Where(v => v.Key > fromPosition)
                .Select(v => new InMemoryEventEnvelope(v.Key, v.Value))
                .OrderBy(e => e.Position);

            return stream;
        }
    }
}
