namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Core.ReadModel;

    public static class RehydrateFromEventStoreExtensions
    {
        public static void RehydrateFromEventStore(this ReadStrategyRegistration parentRegistration, Action<RehydrateFromEventStoreRegistration> action)
        {
            parentRegistration.Services.AddScoped<IReadModelFacade, EventStoreReadModelFacade>();

            action.Invoke(new RehydrateFromEventStoreRegistration(parentRegistration.Services));
        }
    }
}
