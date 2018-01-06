// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryReadCacheExtensions
    {
        public static void InMemoryCache(this ReadModelCacheRegistration parentRegistration)
        {
            ////parentRegistration.Services.TryAddSingleton(typeof(IDatabase<>), typeof(InMemoryDatabase<>));
            parentRegistration.Services.TryAddSingleton<IDatabase<ApplicationListDto>, InMemoryApplicationListDtoDatabase>();
            parentRegistration.Services.TryAddSingleton<IDatabase<ApplicationDetailsDto>, InMemoryApplicationDetailsDtoDatabase>();
            parentRegistration.Services.TryAddSingleton<IDatabase<EnvironmentDetailsDto>, InMemoryEnvironmentDetailsDtoDatabase>();
        }
    }
}
