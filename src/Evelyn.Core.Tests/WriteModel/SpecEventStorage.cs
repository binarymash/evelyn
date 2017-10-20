namespace Evelyn.Core.Tests.WriteModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public class SpecEventStorage : IEventStore
    {
        private readonly IEventPublisher _publisher;

        public SpecEventStorage(IEventPublisher publisher, IList<IEvent> events)
        {
            _publisher = publisher;
            Events = events;
        }

        public IList<IEvent> Events { get; set; }

        public Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var e in events)
            {
                Events.Add(e);
            }

            return Task.WhenAll(events.Select(evt => _publisher.Publish(evt, cancellationToken)));
        }

        public Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            var events = Events.Where(x => x.Id == aggregateId && x.Version > fromVersion);
            return Task.FromResult(events);
        }
    }
}
