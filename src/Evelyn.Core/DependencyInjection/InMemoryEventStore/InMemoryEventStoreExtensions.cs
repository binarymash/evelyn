namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using CQRSlite.Events;
    using Evelyn.Core;

    public static class InMemoryEventStoreExtensions
    {
        public static void InMemory(this EventStoreRegistration parentRegistration, Action<InMemoryEventStoreRegistration> action)
        {
            parentRegistration.Services.AddSingleton<IEventStore, InMemoryEventStore>();

            action.Invoke(new InMemoryEventStoreRegistration(parentRegistration.Services));
        }
    }
}
