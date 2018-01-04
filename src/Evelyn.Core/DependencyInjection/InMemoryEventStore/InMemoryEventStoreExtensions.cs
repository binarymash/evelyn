// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using CQRSlite.Events;
    using Evelyn.Core;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryEventStoreExtensions
    {
        public static void InMemory(this EventStoreRegistration parentRegistration, Action<InMemoryEventStoreRegistration> action)
        {
            parentRegistration.Services.TryAddSingleton<IEventStore, InMemoryEventStore>();

            action.Invoke(new InMemoryEventStoreRegistration(parentRegistration.Services));
        }
    }
}
