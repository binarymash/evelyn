namespace Evelyn.Core.Tests
{
    using CQRSlite.Events;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class SpecEventPublisher : IEventPublisher
    {
        public SpecEventPublisher()
        {
            PublishedEvents = new List<IEvent>();
        }

        public Task Publish<T>(T @event, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IEvent
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public IList<IEvent> PublishedEvents { get; set; }

    }
}
