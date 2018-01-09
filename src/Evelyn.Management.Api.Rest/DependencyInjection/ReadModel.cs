// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ReadModel
    {
        public static void WithReadModel(this EvelynApiOptions parentRegistration, Action<ReadModelOptions> action)
        {
            parentRegistration.Services.AddEvelynReadModel(action);
        }
    }
}
