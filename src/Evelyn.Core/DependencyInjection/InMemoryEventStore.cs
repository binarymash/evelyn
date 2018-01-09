// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Runtime.CompilerServices;
    using CQRSlite.Events;
    using Evelyn.Core;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryEventStore
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// We will use an in-memory event store.
        /// WARNING! Any events stored in here will be lost when the executing process exits, so you
        /// probably don't want to use this in production.
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action">An Action&lt;InMemoryEventStoreOptions&gt; to configure the provided InMemoryEventStoreOptions</param>
        public static void InMemory(this EventStoreOptions parentOptions, Action<InMemoryEventStoreOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.TryAddSingleton<IEventStore, Evelyn.Core.InMemoryEventStore>();

            action.Invoke(new InMemoryEventStoreOptions(parentOptions.Services));
        }
    }
}
