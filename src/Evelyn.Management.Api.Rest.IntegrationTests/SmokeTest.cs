namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Management.Api.Rest.Write.Applications.Messages;
    using Evelyn.Management.Api.Rest.Write.Environments.Messages;
    using Flurl.Http;
    using Microsoft.AspNetCore.Http;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class SmokeTest : IntegrationTest
    {
        private HttpResponseMessage _response;
        private string _responseContent;
        private CreateApplication _createApplicationCommand;
        private AddEnvironment _addEnvironmentCommand;

        [Fact]
        public void NominalTests()
        {
            this.When(_ => WhenGetApplications())
                .Then(_ => ThenTheResponseHasStatusCode200OK())
                .And(_ => ThenTheResponseContentIsAnEmptyCollection())
                .When(_ => WhenAddAnApplication())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())
                .When(_ => WhenAddAnEnvironment())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())
                .BDDfy();
        }

        private async Task WhenGetApplications()
        {
            _response = await Client
                .Request("/api/applications")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenAddAnApplication()
        {
            _createApplicationCommand = DataFixture.Create<CreateApplication>();

            _response = await Client
                .Request("/api/applications")
                .PostJsonAsync(_createApplicationCommand);
        }

        private async Task WhenAddAnEnvironment()
        {
            _addEnvironmentCommand = DataFixture.Create<AddEnvironment>();
            _addEnvironmentCommand.ExpectedVersion = 1;

            _response = await Client
                .Request($"/api/applications/{_createApplicationCommand.Id}/environments")
                .PostJsonAsync(_addEnvironmentCommand);
        }

        private void ThenTheResponseHasStatusCode200OK()
        {
            ThenTheResponseHasStatusCode(StatusCodes.Status200OK);
        }

        private void ThenTheResponseHasStatusCode404NotFound()
        {
            ThenTheResponseHasStatusCode(StatusCodes.Status404NotFound);
        }

        private void ThenTheResponseHasStatusCode202Accepted()
        {
            ThenTheResponseHasStatusCode(StatusCodes.Status202Accepted);
        }

        private void ThenTheResponseHasStatusCode(int expectedStatusCode)
        {
            ((int)_response.StatusCode).ShouldBe(expectedStatusCode);
        }

        private void ThenTheResponseContentIsAnEmptyCollection()
        {
            _responseContent.ShouldBe("[]");
        }
    }
}
