namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System.Net.Http;
    using AutoFixture;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.WriteModel.Handlers;
    using Flurl.Http;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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
                            es.WithEventPublisher.SynchronouslyInProcess();
                        });
                    });
                    api.WithReadModel(rm =>
                    {
                        rm.WithReadStrategy.ReadFromCache(c => c.InMemoryCache());
                    });
                });

                // Register Evelyn event routes
                var registrar = new Core.EvelynRouteRegistrar(new Provider(services.BuildServiceProvider()));
                registrar.RegisterHandlers(
                    typeof(ApplicationCommandHandler),
                    typeof(ApplicationDetailsHandler),
                    typeof(ApplicationListHandler),
                    typeof(EnvironmentDetailsHandler));
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseMvc();
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
