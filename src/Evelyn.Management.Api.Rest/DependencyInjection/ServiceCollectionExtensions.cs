// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEvelynApi(this IServiceCollection services, Action<ApiRegistration> action)
        {
            var swaggerInfo = new Swashbuckle.AspNetCore.Swagger.Info()
            {
                Version = "v0.1",
                Title = "Evelyn Management API",
                Description = "Management API for Evelyn",
            };

            services.AddSwaggerGen(c => c.SwaggerDoc("v0.1", swaggerInfo));

            action.Invoke(new ApiRegistration(services));

            return services;
        }
    }
}
