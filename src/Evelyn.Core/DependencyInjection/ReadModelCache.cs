// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Core.ReadModel;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ReadModelCache
    {
        public static void ReadFromCache(this ReadStrategyOptions parentOptions, Action<ReadModelCacheOptions> action)
        {
            parentOptions.Services.TryAddSingleton<IReadModelFacade, DatabaseReadModelFacade>();

            action.Invoke(new ReadModelCacheOptions(parentOptions.Services));
        }
    }
}
