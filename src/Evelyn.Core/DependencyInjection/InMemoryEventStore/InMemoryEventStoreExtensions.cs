// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Runtime.CompilerServices;
    using CQRSlite.Events;
    using Evelyn.Core;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryEventStoreExtensions
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// Use an in-memory event store.
        /// Any events stored in here will be lost when the executing process exits, so you
        /// probably don't want to use this in production.
        /// </summary>
        /// <param name="parentRegistration"></param>
        /// <param name="action">An Action&lt;InMemoryEventStoreRegistration&gt; to configure the provided InMemoryEventStoreRegistration</param>
        public static void InMemory(this EventStoreRegistration parentRegistration, Action<InMemoryEventStoreRegistration> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentRegistration.Services.TryAddSingleton<IEventStore, InMemoryEventStore>();

            action.Invoke(new InMemoryEventStoreRegistration(parentRegistration.Services));
        }
    }
}
