namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.Projections.EnvironmentState;
    using Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Projections.ToggleDetails;
    using Evelyn.Management.Api.Rest.Write.Environments.Messages;
    using Evelyn.Management.Api.Rest.Write.Projects.Messages;
    using Evelyn.Management.Api.Rest.Write.Toggles.Messages;
    using FluentAssertions;
    using Flurl.Http;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Polly;
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
            this.Given(_ => GivenWeWaitUntilEverythingIsInitialised())

                .When(_ => WhenGetProjects())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheResponseContentIsACollectionWithOneProject())
                .And(_ => ThenTheDefaultSampleProjectWeAddedIsInTheCollection())
                .BDDfy();

                // writing...
            this.When(_ => WhenWeAddAProject())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())
                .BDDfy();

            this.When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())
                .BDDfy();

            this.When(_ => WhenWeAddAToggle())
                .Then(_ => ThenTheResponseHasStatusCode202Accepted())
                .BDDfy();

            // reading...
            this.Given(_ => GivenweWaitAFewSecondsForEventualConsistency())
                .When(_ => WhenGetProjects())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheResponseContentIsACollectionWithTwoProjects())
                .And(_ => ThenTheProjectWeAddedIsInTheCollection())
                .BDDfy();

            this.When(_ => WhenWeGetTheDetailsForTheProjectWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheProjectContainsOneEnvironment())
                .And(_ => ThenTheEnvironmentWeAddedIsOnTheProject())
                .And(_ => ThenTheProjectContainsOneToggle())
                .And(_ => ThenTheToggleWeAddedIsOnTheProject())
                .BDDfy();

            this.When(_ => WhenWeGetTheDetailsForTheEnvironmentWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheEnvironmentWeAddedIsReturned())
                .BDDfy();

            this.When(_ => WhenWeGetTheDetailsForTheToggleWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheToggleWeAddedIsReturned())
                .BDDfy();

            this.When(_ => WhenWeGetTheStateForTheEnvironmentWeAdded())
                .Then(_ => ThenTheResponseHasStatusCode200Ok())
                .And(_ => ThenTheEnvironmentStateContainsOurToggleStates())
                .BDDfy();
        }

        private async Task GivenWeWaitUntilEverythingIsInitialised()
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(60, retryAttempt => TimeSpan.FromMilliseconds(1));

            await policy.ExecuteAsync(async () =>
            {
                var response = await Client
                    .Request("/api/projects")
                    .GetAsync().ConfigureAwait(false);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            });
        }

        private async Task GivenweWaitAFewSecondsForEventualConsistency()
        {
            // TODO: something more deterministic
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        private async Task WhenGetProjects()
        {
            _response = await Client
                .Request("/api/projects")
                .GetAsync().ConfigureAwait(false);

            _responseContent = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private async Task WhenWeAddAProject()
        {
            _createProjectMessage = DataFixture.Create<CreateProject>();

            _response = await Client
                .Request("/api/projects/create")
                .PostJsonAsync(_createProjectMessage).ConfigureAwait(false);

            _responseContent = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private async Task WhenWeAddAnEnvironment()
        {
            _addEnvironmentMessage = DataFixture.Build<AddEnvironment>()
                .With(e => e.Key, TestUtilities.CreateKey(30))
                .With(e => e.ExpectedProjectVersion, 0)
                .Create();

            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.ProjectId}/environments/add")
                .PostJsonAsync(_addEnvironmentMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeAddAToggle()
        {
            _addToggleMessage = DataFixture.Build<AddToggle>()
                .With(e => e.Key, TestUtilities.CreateKey(30))
                .With(e => e.ExpectedProjectVersion, 1)
                .Create();

            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.ProjectId}/toggles/add")
                .PostJsonAsync(_addToggleMessage);

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheProjectWeAdded()
        {
            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.ProjectId}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheEnvironmentWeAdded()
        {
            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.ProjectId}/environments/{_addEnvironmentMessage.Key}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheDetailsForTheToggleWeAdded()
        {
            _response = await Client
                .Request($"/api/projects/{_createProjectMessage.ProjectId}/toggles/{_addToggleMessage.Key}")
                .GetAsync();

            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        private async Task WhenWeGetTheStateForTheEnvironmentWeAdded()
        {
            _response = await Client
                .Request($"/api/states/{_createProjectMessage.ProjectId}/{_addEnvironmentMessage.Key}")
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

        private void ThenTheResponseContentIsACollectionWithOneProject()
        {
            var response = JsonConvert.DeserializeObject<Core.ReadModel.Projections.AccountProjects.Projection>(_responseContent, DeserializeWithPrivateSetters);
            response.Account.Projects.Count().Should().Be(1);
        }

        private void ThenTheResponseContentIsACollectionWithTwoProjects()
        {
            var response = JsonConvert.DeserializeObject<Core.ReadModel.Projections.AccountProjects.Projection>(_responseContent, DeserializeWithPrivateSetters);
            response.Account.Projects.Count().Should().Be(2);
        }

        private void ThenTheDefaultSampleProjectWeAddedIsInTheCollection()
        {
            var projectList = JsonConvert.DeserializeObject<Core.ReadModel.Projections.AccountProjects.Projection>(_responseContent, DeserializeWithPrivateSetters);
            projectList.Account.Projects.First(p => p.Id == Guid.Parse("{8F73D020-96C4-407E-8602-74FD4E2ED08B}"));
        }

        private void ThenTheProjectWeAddedIsInTheCollection()
        {
            var projectList = JsonConvert.DeserializeObject<Core.ReadModel.Projections.AccountProjects.Projection>(_responseContent, DeserializeWithPrivateSetters);
            var project = projectList.Account.Projects.First(p => p.Id == _createProjectMessage.ProjectId);
            project.Name.Should().Be(_createProjectMessage.Name);
        }

        private void ThenTheProjectContainsOneEnvironment()
        {
            var projectDetails = JsonConvert.DeserializeObject<Core.ReadModel.Projections.ProjectDetails.Projection>(_responseContent, DeserializeWithPrivateSetters).Project;
            projectDetails.Environments.Count().Should().Be(1);
        }

        private void ThenTheEnvironmentWeAddedIsOnTheProject()
        {
            var projectDetails = JsonConvert.DeserializeObject<Core.ReadModel.Projections.ProjectDetails.Projection>(_responseContent, DeserializeWithPrivateSetters).Project;
            projectDetails.Environments.Should().Contain(environment =>
                environment.Key == _addEnvironmentMessage.Key);
        }

        private void ThenTheProjectContainsOneToggle()
        {
            var projectDetails = JsonConvert.DeserializeObject<Core.ReadModel.Projections.ProjectDetails.Projection>(_responseContent, DeserializeWithPrivateSetters).Project;
            projectDetails.Toggles.Count().Should().Be(1);
        }

        private void ThenTheToggleWeAddedIsOnTheProject()
        {
            var projectDetails = JsonConvert.DeserializeObject<Core.ReadModel.Projections.ProjectDetails.Projection>(_responseContent, DeserializeWithPrivateSetters).Project;
            projectDetails.Toggles.Should().Contain(toggle =>
                toggle.Key == _addToggleMessage.Key &&
                toggle.Name == _addToggleMessage.Name);
        }

        private void ThenTheEnvironmentWeAddedIsReturned()
        {
            var environmentDetails = JsonConvert.DeserializeObject<Core.ReadModel.Projections.EnvironmentDetails.Projection>(_responseContent, DeserializeWithPrivateSetters).Environment;
            environmentDetails.Key.Should().Be(_addEnvironmentMessage.Key);
            environmentDetails.ProjectId.Should().Be(_createProjectMessage.ProjectId);
        }

        private void ThenTheToggleWeAddedIsReturned()
        {
            var toggleDetails = JsonConvert.DeserializeObject<Core.ReadModel.Projections.ToggleDetails.Projection>(_responseContent, DeserializeWithPrivateSetters).Toggle;
            toggleDetails.Key.Should().Be(_addToggleMessage.Key);
            toggleDetails.Name.Should().Be(_addToggleMessage.Name);
            toggleDetails.ProjectId.Should().Be(_createProjectMessage.ProjectId);
        }

        private void ThenTheEnvironmentStateContainsOurToggleStates()
        {
            var environmentState = JsonConvert.DeserializeObject<Core.ReadModel.Projections.EnvironmentState.Projection>(_responseContent, DeserializeWithPrivateSetters).EnvironmentState;
            var toggleStates = environmentState.ToggleStates.ToList();
            toggleStates.Count.Should().Be(1);
            toggleStates.Exists(ts => ts.Key == _addToggleMessage.Key && ts.Value == default(bool).ToString()).Should().BeTrue();
        }
    }
}
