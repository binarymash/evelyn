// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ReadModel
    {
        public static IServiceCollection AddEvelynReadModel(this IServiceCollection services, Action<ReadModelOptions> action)
        {
            action.Invoke(new ReadModelOptions(services));

            return services;
        }
    }
}
