// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ApiReadModelExtensions
    {
        public static void WithReadModel(this ApiRegistration parentRegistration, Action<ReadModelRegistration> action)
        {
            parentRegistration.Services.AddEvelynReadModel(action);
        }
    }
}
