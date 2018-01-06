namespace Evelyn.Management.Api.Rest
{
    using Evelyn.Core;
    using Microsoft.AspNetCore.Builder;

    public static class EvelynApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEvelynApi(this IApplicationBuilder app)
        {
            var bootstrapper = app.ApplicationServices.GetService(typeof(IRouteRegistrarBootstrapper)) as IRouteRegistrarBootstrapper;
            bootstrapper.Bootstrap(app.ApplicationServices);

            return app;
        }
    }
}
