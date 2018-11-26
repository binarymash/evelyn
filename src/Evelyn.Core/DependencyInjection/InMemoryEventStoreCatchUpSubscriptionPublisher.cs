// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class InMemoryEventStoreCatchUpSubscriptionPublisher
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// A background service running in-process will read an event stream from the in-memory Event Store and
        /// publish the events to any registered handlers. Note that on start-up all events will be read
        /// from in-memory Event Store and republished.
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action"></param>
        public static void RunningInBackgroundService(this InMemoryEventStorePublisherOptions parentOptions, Action<InMemoryEventStoreCatchUpSubscriptionPublisherOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.AddHostedService<Evelyn.Core.ReadModel.EventStream.Subscribers.InMemoryEventStoreCatchUpSubscriber>();
            action.Invoke(new InMemoryEventStoreCatchUpSubscriptionPublisherOptions(parentOptions.Services));
        }
    }
}
