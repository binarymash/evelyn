// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Extensions;
    using Hosting;

    public static class SubscriptionPublisher
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// A background service running in-process will read an event stream from Event Store and
        /// publish the events to any registered handlers. Note that on start-up all events will be read
        /// from Event Store and republished.
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action"></param>
        public static void RunningInBackgroundService(this EventStorePublisherOptions parentOptions, Action<SubscriptionPublisherOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.TryAddSingleton<IHostedService, Evelyn.Storage.EventStore.SubscriptionPublisher>();
            action.Invoke(new SubscriptionPublisherOptions(parentOptions.Services));
        }
    }
}
