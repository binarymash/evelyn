﻿namespace Evelyn.Host
{
    using Core.WriteModel;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();

            services.AddEvelynApi(api =>
            {
                api.WithWriteModel(wm =>
                {
                    ////wm.WithEventStore.InMemory(es =>
                    ////{
                    ////    es.WithEventPublisher.SynchronouslyInProcess();
                    ////});
                    wm.WithEventStore.UsingEventStoreDotOrg(es =>
                    {
                        es.ConnectionFactory = new EventStoreConnectionFactory("tcp://macos:1113");
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Evelyn Management API"));
        }
    }
}
