﻿namespace Evelyn.Server.Host
{
    using Evelyn.Core.WriteModel;
    using Evelyn.Management.Api.Rest;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors();

            services.AddEvelynApi(api =>
            {
                api.WithWriteModel(wm =>
                {
                    // Use a simple in-memory event store...
                    ////wm.WithEventStore.InMemory(es =>
                    ////{
                    ////    es.WithEventPublisher.SynchronouslyInProcess();
                    ////});

                    // ...or,  use Greg Young's EventStore
                    wm.WithEventStore.UsingEventStoreDotOrg(es =>
                    {
                        es.ConnectionFactory = new EventStoreConnectionFactory("tcp://eventstore:1113");
                        es.WithEventPublisher.RunningInBackgroundService(p =>
                        {
                            p.PublishEvents.SynchronouslyInProcess();
                        });
                    });
                });

                api.WithReadModel(rm =>
                {
                    rm.WithReadStrategy.ReadFromCache(c => c.InMemoryCache());
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IStartUpCommands startUpCommands)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMvc();

            app.UseEvelynApi(startUpCommands);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/management-api/swagger.json", "Evelyn Management API");
                c.SwaggerEndpoint("/swagger/client-api/swagger.json", "Evelyn Client API");
            });
        }
    }
}
