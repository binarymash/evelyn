namespace Evelyn.Host
{
    using System;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.WriteModel.Handlers;
    using Evelyn.Management.Api.Rest;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddEvelynApi(api =>
            {
                api.WithWriteModel(wm =>
                {
                    wm.WithEventStore.InMemory(es =>
                    {
                        es.WithEventPublisher.SynchronouslyInProcess();
                    });
                });
                api.WithReadModel(rm =>
                {
                    rm.WithReadStrategy.ReadFromCache(c => c.InMemoryCache());
                });
            });

            var serviceProvider = new Provider(services.BuildServiceProvider());
            var registrar = new Core.EvelynRouteRegistrar(serviceProvider);

            // Register routes
            registrar.RegisterHandlers(
                typeof(ApplicationCommandHandler),
                typeof(ApplicationDetailsHandler),
                typeof(ApplicationListHandler),
                typeof(EnvironmentDetailsHandler));

            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Evelyn Management API"));
        }
    }
}
