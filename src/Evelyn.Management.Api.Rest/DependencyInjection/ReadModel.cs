// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ReadModel
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// Configures what happens when a query is received by the API.
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="action">An Action&lt;WriteModelOptions;gt; to configure the provided ReadModelOptions</param>
        public static void WithReadModel(this EvelynApiOptions parentOptions, Action<ReadModelOptions> action)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.AddEvelynReadModel(action);
        }
    }
}
