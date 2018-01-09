// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class EvelynApi
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
#pragma warning disable SA1616 // Element return value documentation must have text
                              /// <summary>
                              /// Adds Evelyn Management Rest API services to the specified IServiceCollection
                              /// </summary>
                              /// <param name="services"></param>
                              /// <param name="options">An Action&lt;ApiRegistration&gt; to configure the provided ApiRegistration</param>
                              /// <returns></returns>
        public static IServiceCollection AddEvelynApi(this IServiceCollection services, Action<EvelynApiOptions> options)
#pragma warning restore SA1616 // Element return value documentation must have text
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            var swaggerInfo = new Swashbuckle.AspNetCore.Swagger.Info()
            {
                Version = "v0.1",
                Title = "Evelyn Management API",
                Description = "Management API for Evelyn",
            };

            services.AddSwaggerGen(c => c.SwaggerDoc("v0.1", swaggerInfo));

            options.Invoke(new EvelynApiOptions(services));

            return services;
        }
    }
}
