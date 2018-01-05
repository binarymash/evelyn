namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class InProcessStartUp
    {
        public InProcessStartUp(IConfiguration configuration)
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

            return new Provider(services.BuildServiceProvider());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseEvelynApi();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Evelyn Management API"));
        }
    }
}
