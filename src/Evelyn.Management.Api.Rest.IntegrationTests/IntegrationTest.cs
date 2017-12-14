namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System;
    using System.Net.Http;
    using AutoFixture;
    using CQRSlite.Routing;
    using Evelyn.Core.WriteModel.Handlers;
    using Flurl.Http;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class IntegrationTest
    {
        public IntegrationTest()
        {
            DataFixture = new Fixture();
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            FlurlHttp.Configure(c =>
            {
                c.HttpClientFactory = new HttpClientFactory(Server);
            });

            Client = new FlurlClient(Server.BaseAddress.AbsoluteUri);
        }

        protected TestServer Server { get; }

        protected FlurlClient Client { get; }

        protected Fixture DataFixture { get; }

        private class Startup
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
                            es.WithEventPublisher.DoNotPublishEvents();
                        });
                    });
                    api.WithReadModel(rm =>
                    {
                        rm.WithReadStrategy.ReadFromCache(c => c.InMemoryCache());
                    });
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

        private class HttpClientFactory : Flurl.Http.Configuration.DefaultHttpClientFactory
        {
            private TestServer _server;

            public HttpClientFactory(TestServer server)
            {
                _server = server;
            }

            public override HttpClient CreateHttpClient(HttpMessageHandler handler)
            {
                return _server.CreateClient();
            }
        }
    }
}
