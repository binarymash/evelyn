﻿namespace Evelyn.Host
{
    using System.Net;
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

            services.AddEvelynApi(api =>
            {
                api.WithWriteModel(wm =>
                {
                    wm.WithEventStore.InMemory(es =>
                    {
                        es.WithEventPublisher.SynchronouslyInProcess();
                    });
                    ////wm.WithEventStore.UsingEventStoreDotOrg(es =>
                    ////{
                    ////    es.Endpoint = new IPEndPoint(IPAddress.Parse("192.168.1.64"), 2113);
                    ////});
                });
                api.WithReadModel(rm =>
                {
                    rm.WithReadStrategy.ReadFromCache(c => c.InMemoryCache());
                });
            });
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
