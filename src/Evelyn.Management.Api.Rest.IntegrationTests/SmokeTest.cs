namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Management.Api.Rest.Write.Applications.Messages;
    using Evelyn.Management.Api.Rest.Write.Environments.Messages;
    using Flurl.Http;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
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

            // writing...
                .When(_ => WhenAddAnApplication())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())
                .When(_ => WhenAddAnEnvironment())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())

            // reading...
                .When(_ => WhenGetApplications())
                .Then(_ => ThenTheResponseHasStatusCode200OK())
                .And(_ => ThenTheResponseContentIsACollectionWithOneApplication())
                .And(_ => ThenTheApplicationWeAddedIsInTheCollection())
                .When(_ => WhenWeGetTheDetailsForTheApplicationWeAdded())
                .Then(_ => ThenTheApplicationContainsOneEnvironment())
                .And(_ => ThenTheEnvironmentWeAddedIsOnTheApplication())

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

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenAddAnEnvironment()
        {
            _addEnvironmentCommand = DataFixture.Create<AddEnvironment>();
            _addEnvironmentCommand.ExpectedVersion = 1;

            _response = await Client
                .Request($"/api/applications/{_createApplicationCommand.Id}/environments")
                .PostJsonAsync(_addEnvironmentCommand);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheApplicationWeAdded()
        {
            _response = await Client
                .Request($"/api/applications/{_createApplicationCommand.Id}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
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
            var response = JsonConvert.DeserializeObject<List<ApplicationListDto>>(_responseContent, DeserializeWithPrivateSetters);
            response.Count.ShouldBe(0);
        }

        private void ThenTheResponseContentIsACollectionWithOneApplication()
        {
            var response = JsonConvert.DeserializeObject<List<ApplicationListDto>>(_responseContent, DeserializeWithPrivateSetters);
            response.Count.ShouldBe(1);
        }

        private void ThenTheApplicationWeAddedIsInTheCollection()
        {
            var applicationList = JsonConvert.DeserializeObject<List<ApplicationListDto>>(_responseContent, DeserializeWithPrivateSetters).ToList();
            applicationList.ShouldContain(application => application.Id == _createApplicationCommand.Id);
        }

        private void ThenTheApplicationContainsOneEnvironment()
        {
            var applicationDetails = JsonConvert.DeserializeObject<ApplicationDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            applicationDetails.Environments.Count().ShouldBe(1);
        }

        private void ThenTheEnvironmentWeAddedIsOnTheApplication()
        {
            var applicationDetails = JsonConvert.DeserializeObject<ApplicationDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            applicationDetails.Environments.ShouldContain(environment => environment.Id == _addEnvironmentCommand.Id);
        }
    }
}
