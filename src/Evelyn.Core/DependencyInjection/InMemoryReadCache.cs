// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.Infrastructure;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryReadCache
    {
        public static void InMemoryCache(this ReadModelCacheOptions parentOptions)
        {
            parentOptions.Services.TryAddSingleton(typeof(IDatabase<>), typeof(InMemoryDatabase<>));
        }
    }
}
