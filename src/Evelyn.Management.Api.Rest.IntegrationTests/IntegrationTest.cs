namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System.Net.Http;
    using AutoFixture;
    using Flurl.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;

    public abstract class IntegrationTest
    {
        protected IntegrationTest()
        {
            DataFixture = new Fixture();
            Server = new TestServer(new WebHostBuilder().UseStartup<InProcessStartUp>());

            FlurlHttp.Configure(c =>
            {
                c.HttpClientFactory = new HttpClientFactory(Server);
            });

            Client = new FlurlClient(Server.BaseAddress.AbsoluteUri);
        }

        protected TestServer Server { get; }

        protected FlurlClient Client { get; }

        protected Fixture DataFixture { get; }

        private class HttpClientFactory : Flurl.Http.Configuration.DefaultHttpClientFactory
        {
            private readonly TestServer _server;

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
