// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class WriteModel
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// Configures what will happen when a command is received by the API.
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action">An Action&lt;WriteModelOptions;gt; to configure the provided WriteModelOptions</param>
        public static void WithWriteModel(this EvelynApiOptions parentOptions, Action<WriteModelOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.AddEvelynWriteModel(action);
        }
    }
}
