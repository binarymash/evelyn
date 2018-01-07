// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.Infrastructure;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryReadCacheExtensions
    {
        public static void InMemoryCache(this ReadModelCacheRegistration parentRegistration)
        {
            parentRegistration.Services.TryAddSingleton(typeof(IDatabase<>), typeof(InMemoryDatabase<>));
        }
    }
}
