namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Core.ReadModel.ProjectDetails;
    using Core.ReadModel.ProjectList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using Evelyn.Management.Api.Rest.Write.Environments.Messages;
    using Evelyn.Management.Api.Rest.Write.Projects.Messages;
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
        private CreateProject _createProjectMessage;
        private AddEnvironment _addEnvironmentMessage;
        private AddToggle _addToggleMessage;

        [Fact]
        public void NominalTests()
        {
            this.When(_ => WhenGetProjects())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheResponseContentIsAnEmptyCollection())

            // writing...
                .When(_ => WhenWeAddAProject())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())

                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())

                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())

            // reading...
                .When(_ => WhenGetProjects())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheResponseContentIsACollectionWithOneProject())
                .And(_ => ThenTheProjectWeAddedIsInTheCollection())

                .When(_ => WhenWeGetTheDetailsForTheProjectWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheProjectContainsOneEnvironment())
                .And(_ => ThenTheEnvironmentWeAddedIsOnTheProject())
                .And(_ => ThenTheProjectContainsOneToggle())
                .And(_ => ThenTheToggleWeAddedIsOnTheProject())

                .When(_ => WhenWeGetTheDetailsForTheEnvironmentWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheEnvironmentWeAddedIsReturned())

                .When(_ => WhenWeGetTheDetailsForTheToggleWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheToggleWeAddedIsReturned())
                .BDDfy();
        }

        private async Task WhenGetProjects()
        {
            _response = await Client
                .Request("/api/projects")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeAddAProject()
        {
            _createProjectMessage = DataFixture.Create<CreateProject>();

            _response = await Client
                .Request("/api/projects")
                .PostJsonAsync(_createProjectMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeAddAnEnvironment()
        {
            _addEnvironmentMessage = DataFixture.Create<AddEnvironment>();
            _addEnvironmentMessage.ExpectedVersion = 0;

            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.Id}/environments")
                .PostJsonAsync(_addEnvironmentMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeAddAToggle()
        {
            _addToggleMessage = DataFixture.Create<AddToggle>();
            _addToggleMessage.ExpectedVersion = 1;

            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.Id}/toggles")
                .PostJsonAsync(_addToggleMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheProjectWeAdded()
        {
            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.Id}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheEnvironmentWeAdded()
        {
            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.Id}/environments/{_addEnvironmentMessage.Key}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheToggleWeAdded()
        {
            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.Id}/toggles/{_addToggleMessage.Key}")
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
            var response = JsonConvert.DeserializeObject<AccountProjectsDto>(_responseContent, DeserializeWithPrivateSetters);
            response.Projects.Count.Should().Be(0);
        }

        private void ThenTheResponseContentIsACollectionWithOneProject()
        {
            var response = JsonConvert.DeserializeObject<AccountProjectsDto>(_responseContent, DeserializeWithPrivateSetters);
            response.Projects.Count.Should().Be(1);
        }

        private void ThenTheProjectWeAddedIsInTheCollection()
        {
            var projectList = JsonConvert.DeserializeObject<AccountProjectsDto>(_responseContent, DeserializeWithPrivateSetters);
            projectList.Projects[_createProjectMessage.Id].Id.Should().Be(_createProjectMessage.Id);
            projectList.Projects[_createProjectMessage.Id].Name.Should().Be(_createProjectMessage.Name);
        }

        private void ThenTheProjectContainsOneEnvironment()
        {
            var projectDetails = JsonConvert.DeserializeObject<ProjectDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            projectDetails.Environments.Count().Should().Be(1);
        }

        private void ThenTheEnvironmentWeAddedIsOnTheProject()
        {
            var projectDetails = JsonConvert.DeserializeObject<ProjectDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            projectDetails.Environments.Should().Contain(environment =>
                environment.Key == _addEnvironmentMessage.Key);
        }

        private void ThenTheProjectContainsOneToggle()
        {
            var projectDetails = JsonConvert.DeserializeObject<ProjectDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            projectDetails.Toggles.Count().Should().Be(1);
        }

        private void ThenTheToggleWeAddedIsOnTheProject()
        {
            var projectDetails = JsonConvert.DeserializeObject<ProjectDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            projectDetails.Toggles.Should().Contain(toggle =>
                toggle.Key == _addToggleMessage.Key &&
                toggle.Name == _addToggleMessage.Name);
        }

        private void ThenTheEnvironmentWeAddedIsReturned()
        {
            var environmentDetails = JsonConvert.DeserializeObject<EnvironmentDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            environmentDetails.Key.Should().Be(_addEnvironmentMessage.Key);
            environmentDetails.ProjectId.Should().Be(_createProjectMessage.Id);
        }

        private void ThenTheToggleWeAddedIsReturned()
        {
            var toggleDetails = JsonConvert.DeserializeObject<ToggleDetailsDto>(_responseContent, DeserializeWithPrivateSetters);
            toggleDetails.Key.Should().Be(_addToggleMessage.Key);
            toggleDetails.Name.Should().Be(_addToggleMessage.Name);
            toggleDetails.ProjectId.Should().Be(_createProjectMessage.Id);
        }
    }
}
