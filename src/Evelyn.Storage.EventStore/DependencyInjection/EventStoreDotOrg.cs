// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using CQRSlite.Events;
    using Evelyn.Storage.EventStore;
    using Hosting;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class EventStoreDotOrg
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// We will use Event Store to store our events. See https://eventstore.org
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action">An Action&lt;EventStoreDotOrgOptions&gt; to configure the provided InMemoryEventStoreOptions</param>
        public static void UsingEventStoreDotOrg(this EventStoreOptions parentOptions, Action<EventStoreDotOrgOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.TryAddSingleton<IEventStore, EventStoreImplementation>();

            var options = new EventStoreDotOrgOptions(parentOptions.Services);
            action.Invoke(options);
            parentOptions.Services.TryAddSingleton(options.ConnectionFactory);
        }
    }
}
