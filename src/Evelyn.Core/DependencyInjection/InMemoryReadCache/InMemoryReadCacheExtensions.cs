namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.Infrastructure;

    public static class InMemoryReadCacheExtensions
    {
        public static void InMemoryCache(this ReadModelCacheRegistration parentRegistration)
        {
            parentRegistration.Services.AddScoped(typeof(IDatabase<>), typeof(InMemoryDatabase<>));
        }
    }
}
