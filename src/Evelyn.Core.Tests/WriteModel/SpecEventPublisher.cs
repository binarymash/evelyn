namespace Evelyn.Core.Tests.WriteModel
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public class SpecEventPublisher : IEventPublisher
    {
        public SpecEventPublisher()
        {
            PublishedEvents = new List<IEvent>();
        }

        public IList<IEvent> PublishedEvents { get; set; }

        public Task Publish<T>(T @event, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IEvent
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }
    }
}
