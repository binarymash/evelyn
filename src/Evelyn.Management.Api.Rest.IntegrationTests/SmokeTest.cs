namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using Evelyn.Management.Api.Rest.Write.Applications.Messages;
    using Evelyn.Management.Api.Rest.Write.Environments.Messages;
    using Evelyn.Management.Api.Rest.Write.Toggles.Messages;
    using FluentAssertions;
    using Flurl.Http;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using TestStack.BDDfy;
    using Xunit;

    public class SmokeTest : IntegrationTest
    {
        private HttpResponseMessage _response;
        private string _responseContent;
        private CreateApplication _createApplicationMessage;
        private AddEnvironment _addEnvironmentMessage;
        private AddToggle _addToggleMessage;

        [Fact]
        public void NominalTests()
        {
            this.When(_ => WhenGetApplications())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheResponseContentIsAnEmptyCollection())

            // writing...
                .When(_ => WhenWeAddAnApplication())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())

                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())

                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())

            // reading...
                .When(_ => WhenGetApplications())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheResponseContentIsACollectionWithOneApplication())
                .And(_ => ThenTheApplicationWeAddedIsInTheCollection())

                .When(_ => WhenWeGetTheDetailsForTheApplicationWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheApplicationContainsOneEnvironment())
                .And(_ => ThenTheEnvironmentWeAddedIsOnTheApplication())
                .And(_ => ThenTheApplicationContainsOneToggle())
                .And(_ => ThenTheToggleWeAddedIsOnTheApplication())

                .When(_ => WhenWeGetTheDetailsForTheEnvironmentWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheEnvironmentWeAddedIsReturned())

                .When(_ => WhenWeGetTheDetailsForTheToggleWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheToggleWeAddedIsReturned())
                .BDDfy();
        }

        private async Task WhenGetApplications()
        {
            _response = await Client
                .Request("/api/applications")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeAddAnApplication()
        {
            _createApplicationMessage = DataFixture.Create<CreateApplication>();

            _response = await Client
                .Request("/api/applications")
                .PostJsonAsync(_createApplicationMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeAddAnEnvironment()
        {
            _addEnvironmentMessage = DataFixture.Create<AddEnvironment>();
            _addEnvironmentMessage.ExpectedVersion = 1;

            _response = await Client
                .Request($"/api/applications/{_createApplicationMessage.Id}/environments")
                .PostJsonAsync(_addEnvironmentMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeAddAToggle()
        {
            _addToggleMessage = DataFixture.Create<AddToggle>();
            _addToggleMessage.ExpectedVersion = 2;

            _response = await Client
                .Request($"/api/applications/{_createApplicationMessage.Id}/toggles")
                .PostJsonAsync(_addToggleMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheApplicationWeAdded()
        {
            _response = await Client
                .Request($"/api/applications/{_createApplicationMessage.Id}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheEnvironmentWeAdded()
        {
            _response = await Client
                .Request($"/api/applications/{_createApplicationMessage.Id}/environments/{_addEnvironmentMessage.Id}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheToggleWeAdded()
        {
            _response = await Client
                .Request($"/api/applications/{_createApplicationMessage.Id}/toggles/{_addToggleMessage.Id}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private void ThenTheResponseHasStatusCode200Ok()
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
            ((int)_response.StatusCode).Should().Be(expectedStatusCode);
        }

        private void ThenTheResponseContentIsAnEmptyCollection()
        {
            var response = JsonConvert.DeserializeObject<List<ApplicationListDto>>(_responseContent, DeserializeWithPrivateSetters);
            response.Count.Should().Be(0);
        }

        private void ThenTheResponseContentIsACollectionWithOneApplication()
        {
            var response = JsonConvert.DeserializeObject<List<ApplicationListDto>>(_responseContent, DeserializeWithPrivateSetters);
            response.Count.Should().Be(1);
        }

        private void ThenTheApplicationWeAddedIsInTheCollection()
        {
            var applicationList = JsonConvert.DeserializeObject<List<ApplicationListDto>>(_responseContent, DeserializeWithPrivateSetters).ToList();
            applicationList.Should().Contain(application =>
                application.Id == _createApplicationMessage.Id &&
                application.Name == _createApplicationMessage.Name);
        }

        private void ThenTheApplicationContainsOneEnvironment()
        {
            var applicationDetails = JsonConvert.DeserializeObject<ApplicationDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            applicationDetails.Environments.Count().Should().Be(1);
        }

        private void ThenTheEnvironmentWeAddedIsOnTheApplication()
        {
            var applicationDetails = JsonConvert.DeserializeObject<ApplicationDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            applicationDetails.Environments.Should().Contain(environment =>
                environment.Id == _addEnvironmentMessage.Id &&
                environment.Name == _addEnvironmentMessage.Name);
        }

        private void ThenTheApplicationContainsOneToggle()
        {
            var applicationDetails = JsonConvert.DeserializeObject<ApplicationDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            applicationDetails.Toggles.Count().Should().Be(1);
        }

        private void ThenTheToggleWeAddedIsOnTheApplication()
        {
            var applicationDetails = JsonConvert.DeserializeObject<ApplicationDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            applicationDetails.Toggles.Should().Contain(toggle =>
                toggle.Id == _addToggleMessage.Id &&
                toggle.Name == _addToggleMessage.Name);
        }

        private void ThenTheEnvironmentWeAddedIsReturned()
        {
            var environmentDetails = JsonConvert.DeserializeObject<EnvironmentDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            environmentDetails.Id.Should().Be(_addEnvironmentMessage.Id);
            environmentDetails.Name.Should().Be(_addEnvironmentMessage.Name);
            environmentDetails.ApplicationId.Should().Be(_createApplicationMessage.Id);
        }

        private void ThenTheToggleWeAddedIsReturned()
        {
            var toggleDetails = JsonConvert.DeserializeObject<ToggleDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            toggleDetails.Id.Should().Be(_addToggleMessage.Id);
            toggleDetails.Name.Should().Be(_addToggleMessage.Name);
            toggleDetails.Key.Should().Be(_addToggleMessage.Key);
            toggleDetails.ApplicationId.Should().Be(_createApplicationMessage.Id);
        }
    }
}
