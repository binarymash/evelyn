// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Core.ReadModel;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ReadModelCacheExtensions
    {
        public static void ReadFromCache(this ReadStrategyRegistration parentRegistration, Action<ReadModelCacheRegistration> action)
        {
            parentRegistration.Services.TryAddSingleton<IReadModelFacade, DatabaseReadModelFacade>();

            action.Invoke(new ReadModelCacheRegistration(parentRegistration.Services));
        }
    }
}
