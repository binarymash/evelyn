// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ReadModelExtensions
    {
        public static IServiceCollection AddEvelynReadModel(this IServiceCollection services, Action<ReadModelRegistration> action)
        {
            action.Invoke(new ReadModelRegistration(services));

            return services;
        }
    }
}
