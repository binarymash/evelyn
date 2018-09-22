namespace Evelyn.Management.Api.Rest
{
    using Core.WriteModel;
    using Evelyn.Core;
    using Microsoft.AspNetCore.Builder;

    public static class EvelynApplicationBuilderExtensions
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
#pragma warning disable SA1616 // Element return value documentation must have text
        /// <summary>
        /// Adds the Evelyn Management ReST API to the application
        /// </summary>
        /// <param name="app"></param>
        /// <param name="startUpCommands">Optional startup behaviour.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseEvelynApi(this IApplicationBuilder app, IStartUpCommands startUpCommands)
#pragma warning restore SA1616 // Element return value documentation must have text
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            var bootstrapper = app.ApplicationServices.GetService(typeof(IBootstrapper)) as IBootstrapper;
            bootstrapper.Bootstrap(app.ApplicationServices, startUpCommands);

            return app;
        }
    }
}
