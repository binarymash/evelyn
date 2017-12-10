namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Core.ReadModel;

    public static class ReadModelCacheExtensions
    {
        public static void ReadFromCache(this ReadStrategyRegistration parentRegistration, Action<ReadModelCacheRegistration> action)
        {
            parentRegistration.Services.AddScoped<IReadModelFacade, DatabaseReadModelFacade>();

            action.Invoke(new ReadModelCacheRegistration(parentRegistration.Services));
        }
    }
}
