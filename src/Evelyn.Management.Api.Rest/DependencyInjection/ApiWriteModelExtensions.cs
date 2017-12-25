// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ApiWriteModelExtensions
    {
        public static void WithWriteModel(this ApiRegistration parentRegistration, Action<WriteModelRegistration> action)
        {
            parentRegistration.Services.AddEvelynWriteModel(action);
        }
    }
}
