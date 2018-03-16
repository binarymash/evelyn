namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Events;

    public class StubbedRepository : IRepository
    {
        private readonly List<IEvent> _events;
        private readonly Dictionary<Guid, Func<AggregateRoot>> _aggregateFactories;

        public StubbedRepository()
        {
            _events = new List<IEvent>();
            _aggregateFactories = new Dictionary<Guid, Func<AggregateRoot>>();
        }

        public IEnumerable<IEvent> Events => _events.ToList();

        public IEnumerable<IEvent> EventsFor(Guid aggregateId)
        {
            return _events.Where(e => e.Id == aggregateId);
        }

        public int NextVersionFor(Guid aggregateId)
        {
            return EventsFor(aggregateId).Count();
        }

        public void AddEvent(IEvent @event, Func<AggregateRoot> factory = null)
        {
            _events.Add(@event);

            if (factory != null)
            {
                _aggregateFactories.Add(@event.Id, factory);
            }
        }

        public Task Save<T>(T aggregate, int? expectedVersion = null, CancellationToken cancellationToken = new CancellationToken())
            where T : AggregateRoot
        {
            throw new NotImplementedException();
        }

        public Task<T> Get<T>(Guid aggregateId, CancellationToken cancellationToken = new CancellationToken())
            where T : AggregateRoot
        {
            var aggregate = _aggregateFactories[aggregateId]() as T;
            var events = _events.Where(e => e.Id == aggregateId).OrderBy(e => e.Version);
            aggregate.LoadFromHistory(events);
            return Task.FromResult(aggregate);
        }
    }
}
