// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class EventStoreCatchUpSubscriptionPublisher
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// A background service running in-process will read an event stream from Event Store and
        /// publish the events to any registered handlers. Note that on start-up all events will be read
        /// from Event Store and republished.
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action"></param>
        public static void RunningInBackgroundService(this EventStorePublisherOptions parentOptions, Action<EventStoreCatchUpSubscriptionPublisherOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.AddHostedService<Evelyn.Storage.EventStore.CatchUpSubscriptionPublisher>();
            action.Invoke(new EventStoreCatchUpSubscriptionPublisherOptions(parentOptions.Services));
        }
    }
}
