// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class WriteModel
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// Configures what happens when a command is receieved by the API
        /// </summary>
        /// <param name="parentRegistration"></param>
        /// <param name="action">An Action&lt;WriteModelRegistration;gt; to configure the provided WriteModelRegistration</param>
        public static void WithWriteModel(this EvelynApiOptions parentRegistration, Action<WriteModelOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentRegistration.Services.AddEvelynWriteModel(action);
        }
    }
}
