// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Core.ReadModel;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ReadModelCache
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// We will get the read model from a pre-populated cache.
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action">An Action&lt;ReadModelCacheOptions&gt; to configure the provided ReadModelCacheOptions</param>
        public static void ReadFromCache(this ReadStrategyOptions parentOptions, Action<ReadModelCacheOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.TryAddSingleton<IReadModelFacade, DatabaseReadModelFacade>();

            action.Invoke(new ReadModelCacheOptions(parentOptions.Services));
        }
    }
}
