// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.Infrastructure;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryReadCache
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// Our read model cache will be a memory cache.
        /// </summary>
        /// <param name="parentOptions"></param>
        public static void InMemoryCache(this ReadModelCacheOptions parentOptions)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.TryAddSingleton(typeof(IProjectionStore<,>), typeof(InMemoryProjectionStore<,>));
        }
    }
}
