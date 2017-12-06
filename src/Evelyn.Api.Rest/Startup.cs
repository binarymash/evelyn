﻿namespace Evelyn.Api.Rest
{
    using System;
    using CQRSlite.Routing;
    using Evelyn.Core.WriteModel.Handlers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var swaggerInfo = new Swashbuckle.AspNetCore.Swagger.Info()
            {
                Version = "v0.1",
                Title = "Evelyn Management API",
                Description = "Management API for Evelyn",
            };

            services.AddSwaggerGen(c => c.SwaggerDoc("v0.1", swaggerInfo));

            services.AddEvelynWriteModel(wm =>
            {
                wm.WithEventStore.InMemory(es =>
                {
                    es.WithEventPublisher.DoNotPublishEvents();
                });
            });

            services.AddEvelynReadModel(rm =>
            {
                rm.WithReadStrategy.ReadFromCache(c => c.InMemoryCache());
            });

            // Register routes
            var serviceProvider = services.BuildServiceProvider();

            var registrar = new RouteRegistrar(new Provider(serviceProvider));
            registrar.Register(typeof(ApplicationCommandHandler));
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

        public class Provider : IServiceProvider
        {
            private readonly ServiceProvider _serviceProvider;
            private readonly IHttpContextAccessor _contextAccessor;

            public Provider(ServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
                _contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            }

            public object GetService(Type serviceType)
            {
                return _contextAccessor?.HttpContext?.RequestServices.GetService(serviceType) ??
                       _serviceProvider.GetService(serviceType);
            }
        }
    }
}
