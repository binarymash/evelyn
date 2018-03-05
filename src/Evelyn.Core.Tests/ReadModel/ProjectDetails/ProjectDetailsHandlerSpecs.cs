namespace Evelyn.Core.Tests.ReadModel.ProjectDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.ProjectDetails;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectDetailsHandlerSpecs : HandlerSpecs
    {
        private readonly List<IEvent> _eventsProject1;
        private readonly List<IEvent> _eventsProject2;

        private ProjectCreated _project1Created;
        private ProjectCreated _project2Created;

        private EnvironmentAdded _environment1Added;
        private EnvironmentAdded _environment2Added;
        private EnvironmentAdded _environment3Added;

        private ToggleAdded _toggle1Added;
        private ToggleAdded _toggle2Added;
        private ToggleAdded _toggle3Added;

        private Guid _project1Id;
        private Guid _project2Id;

        private ProjectDetailsDto _retrievedProjectDetails;

        public ProjectDetailsHandlerSpecs()
        {
            _eventsProject1 = new List<IEvent>();
            _eventsProject2 = new List<IEvent>();
        }

        [Fact]
        public void ProjectDoesntExist()
        {
            this.Given(_ => GivenThatWeDontCreateProject1())
                .When(_ => WhenWeGetTheDetailsForProject1())
                .Then(_ => ThenANotFoundExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void OneProjectCreated()
        {
            this.Given(_ => GivenProject1IsCreated())
                .When(_ => WhenWeGetTheDetailsForProject1())
                .Then(_ => ThenTheDetailsAreSetForProject1())
                .And(_ => ThenThereAreNoEnvironmentsOnTheProject())
                .And(_ => ThenThereAreNoTogglesOnTheProject())
                .BDDfy();
        }

        [Fact]
        public void MultipleProjectCreated()
        {
            this.Given(_ => GivenProject1IsCreated())
                .And(_ => GivenProject2IsCreated())
                .When(_ => WhenWeGetTheDetailsForProject1())
                .Then(_ => ThenTheDetailsAreSetForProject1())
                .And(_ => ThenThereAreNoEnvironmentsOnTheProject())
                .And(_ => ThenThereAreNoTogglesOnTheProject())
                .When(_ => WhenWeGetTheDetailsForProject2())
                .Then(_ => ThenTheDetailsAreSetForForProject2())
                .And(_ => ThenThereAreNoEnvironmentsOnTheProject())
                .And(_ => ThenThereAreNoTogglesOnTheProject())
                .BDDfy();
        }

        [Fact]
        public void AddingEnvironmentsToProject()
        {
            this.Given(_ => GivenProject1IsCreated())
                .And(_ => GivenProject2IsCreated())
                .And(_ => GivenWeAddEnvironment1ToProject1())
                .And(_ => GivenWeAddEnvironment2ToProject2())
                .And(_ => GivenWeAddEnvironment3ToProject1())
                .When(_ => WhenWeGetTheDetailsForProject1())
                .Then(_ => ThenThereAreTwoEnvironmentsOnTheProject())
                .And(_ => ThenEnvrionment1IsOnTheProject())
                .And(_ => ThenEnvrionment3IsOnTheProject())
                .And(_ => ThenTheVersionOfTheProjectHasBeenUpdatedForEnvironment3())
                .And(_ => ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForEnvironment3())
                .When(_ => WhenWeGetTheDetailsForProject2())
                .Then(_ => ThenThereIsOneEnvironmentOnTheProject())
                .And(_ => ThenEnvironment2IsOnTheProject())
                .And(_ => ThenTheVersionOfTheProjectHasBeenUpdatedForEnvironment2())
                .And(_ => ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForEnvironment2())
                .BDDfy();
        }

        [Fact]
        public void AddingTogglesToProject()
        {
            this.Given(_ => GivenProject1IsCreated())
                .And(_ => GivenProject2IsCreated())
                .And(_ => GivenWeAddToggle1ToProject1())
                .And(_ => GivenWeAddToggle2ToProject2())
                .And(_ => GivenWeAddToggle3ToProject1())
                .When(_ => WhenWeGetTheDetailsForProject1())
                .Then(_ => ThenThereAreTwoTogglesOnTheProject())
                .And(_ => ThenToggle1IsOnTheProject())
                .And(_ => ThenToggle3IsOnTheProject())
                .And(_ => ThenTheVersionOfTheProjectHasBeenUpdatedForToggle3())
                .And(_ => ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForToggle3())
                .When(_ => WhenWeGetTheDetailsForProject2())
                .Then(_ => ThenThereIsOneToggleOnTheProject())
                .And(_ => ThenToggle2IsOnTheProject())
                .And(_ => ThenTheVersionOfTheProjectHasBeenUpdatedForToggle2())
                .And(_ => ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForToggle2())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new ProjectDetailsHandler(ProjectDetailsStore);
            router.RegisterHandler<ProjectCreated>(handler.Handle);
            router.RegisterHandler<EnvironmentAdded>(handler.Handle);
            router.RegisterHandler<ToggleAdded>(handler.Handle);
        }

        private void GivenThatWeDontCreateProject1()
        {
            _project1Id = DataFixture.Create<Guid>();
        }

        private void GivenProject1IsCreated()
        {
            _project1Created = DataFixture.Create<ProjectCreated>();
            _project1Created.Version = _eventsProject1.Count + 1;
            _project1Created.TimeStamp = DateTimeOffset.UtcNow;

            _project1Id = _project1Created.Id;
            _eventsProject1.Add(_project1Created);

            GivenWePublish(_project1Created);
        }

        private void GivenProject2IsCreated()
        {
            _project2Created = DataFixture.Create<ProjectCreated>();
            _project2Created.Version = _eventsProject2.Count + 1;
            _project2Created.TimeStamp = DateTimeOffset.UtcNow;

            _project2Id = _project2Created.Id;
            _eventsProject2.Add(_project2Created);

            GivenWePublish(_project2Created);
        }

        private void GivenWeAddEnvironment1ToProject1()
        {
            _environment1Added = DataFixture.Create<EnvironmentAdded>();
            _environment1Added.Id = _project1Id;
            _environment1Added.Version = _eventsProject1.Count + 1;
            _environment1Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsProject1.Add(_environment1Added);

            GivenWePublish(_environment1Added);
        }

        private void GivenWeAddEnvironment2ToProject2()
        {
            _environment2Added = DataFixture.Create<EnvironmentAdded>();
            _environment2Added.Id = _project2Id;
            _environment2Added.Version = _eventsProject2.Count + 1;
            _environment2Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsProject2.Add(_environment2Added);

            GivenWePublish(_environment2Added);
        }

        private void GivenWeAddEnvironment3ToProject1()
        {
            _environment3Added = DataFixture.Create<EnvironmentAdded>();
            _environment3Added.Id = _project1Id;
            _environment3Added.Version = _eventsProject1.Count + 1;
            _environment3Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsProject1.Add(_environment3Added);

            GivenWePublish(_environment3Added);
        }

        private void GivenWeAddToggle1ToProject1()
        {
            _toggle1Added = DataFixture.Create<ToggleAdded>();
            _toggle1Added.Id = _project1Id;
            _toggle1Added.Version = _eventsProject1.Count + 1;
            _toggle1Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsProject1.Add(_toggle1Added);

            GivenWePublish(_toggle1Added);
        }

        private void GivenWeAddToggle2ToProject2()
        {
            _toggle2Added = DataFixture.Create<ToggleAdded>();
            _toggle2Added.Id = _project2Id;
            _toggle2Added.Version = _eventsProject2.Count + 1;
            _toggle2Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsProject2.Add(_toggle2Added);

            GivenWePublish(_toggle2Added);
        }

        private void GivenWeAddToggle3ToProject1()
        {
            _toggle3Added = DataFixture.Create<ToggleAdded>();
            _toggle3Added.Id = _project1Id;
            _toggle3Added.Version = _eventsProject1.Count + 1;
            _toggle3Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsProject1.Add(_toggle3Added);

            GivenWePublish(_toggle3Added);
        }

        private async Task WhenWeGetTheDetailsForProject1()
        {
            await WhenWeGetTheDetailsFor(_project1Id);
        }

        private async Task WhenWeGetTheDetailsForProject2()
        {
            await WhenWeGetTheDetailsFor(_project2Id);
        }

        private async Task WhenWeGetTheDetailsFor(Guid projectId)
        {
            try
            {
                _retrievedProjectDetails = await ReadModelFacade.GetProjectDetails(projectId);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenANotFoundExceptionIsThrown()
        {
            ThrownException.Should().BeOfType<NotFoundException>();
        }

        private void ThenThereAreNoEnvironmentsOnTheProject()
        {
            _retrievedProjectDetails.Environments.Count().Should().Be(0);
        }

        private void ThenThereAreTwoEnvironmentsOnTheProject()
        {
            _retrievedProjectDetails.Environments.Count().Should().Be(2);
        }

        private void ThenThereIsOneEnvironmentOnTheProject()
        {
            _retrievedProjectDetails.Environments.Count().Should().Be(1);
        }

        private void ThenEnvrionment1IsOnTheProject()
        {
            ThenTheEnvironmentIsOnTheProject(_environment1Added);
        }

        private void ThenEnvironment2IsOnTheProject()
        {
            ThenTheEnvironmentIsOnTheProject(_environment2Added);
        }

        private void ThenEnvrionment3IsOnTheProject()
        {
            ThenTheEnvironmentIsOnTheProject(_environment3Added);
        }

        private void ThenThereAreNoTogglesOnTheProject()
        {
            _retrievedProjectDetails.Toggles.Count().Should().Be(0);
        }

        private void ThenThereAreTwoTogglesOnTheProject()
        {
            _retrievedProjectDetails.Toggles.Count().Should().Be(2);
        }

        private void ThenThereIsOneToggleOnTheProject()
        {
            _retrievedProjectDetails.Toggles.Count().Should().Be(1);
        }

        private void ThenToggle1IsOnTheProject()
        {
            ThenTheToggleIsOnTheProject(_toggle1Added);
        }

        private void ThenToggle2IsOnTheProject()
        {
            ThenTheToggleIsOnTheProject(_toggle2Added);
        }

        private void ThenToggle3IsOnTheProject()
        {
            ThenTheToggleIsOnTheProject(_toggle3Added);
        }

        private void ThenTheDetailsAreSetForProject1()
        {
            ThenTheDetailsAreSetFor(_project1Created);
        }

        private void ThenTheDetailsAreSetForForProject2()
        {
            ThenTheDetailsAreSetFor(_project2Created);
        }

        private void ThenTheDetailsAreSetFor(ProjectCreated ev)
        {
            _retrievedProjectDetails.Id.Should().Be(ev.Id);
            _retrievedProjectDetails.Name.Should().Be(ev.Name);
            _retrievedProjectDetails.Version.Should().Be(ev.Version);
            _retrievedProjectDetails.Created.Should().Be(ev.TimeStamp);
            _retrievedProjectDetails.LastModified.Should().Be(ev.TimeStamp);
        }

        private void ThenTheEnvironmentIsOnTheProject(EnvironmentAdded environmentAdded)
        {
            _retrievedProjectDetails.Environments.Should().Contain(environment =>
                environment.Id == environmentAdded.EnvironmentId &&
                environment.Name == environmentAdded.Name);
        }

        private void ThenTheToggleIsOnTheProject(ToggleAdded toggleAdded)
        {
            _retrievedProjectDetails.Toggles.Should().Contain(toggle =>
                toggle.Id == toggleAdded.ToggleId &&
                toggle.Name == toggleAdded.Name);
        }

        private void ThenTheVersionOfTheProjectHasBeenUpdatedForEnvironment3()
        {
            ThenTheVersionOfTheProjectHasBeenUpdatedTo(_environment3Added.Version);
        }

        private void ThenTheVersionOfTheProjectHasBeenUpdatedForEnvironment2()
        {
            ThenTheVersionOfTheProjectHasBeenUpdatedTo(_environment2Added.Version);
        }

        private void ThenTheVersionOfTheProjectHasBeenUpdatedForToggle3()
        {
            ThenTheVersionOfTheProjectHasBeenUpdatedTo(_toggle3Added.Version);
        }

        private void ThenTheVersionOfTheProjectHasBeenUpdatedForToggle2()
        {
            ThenTheVersionOfTheProjectHasBeenUpdatedTo(_toggle2Added.Version);
        }

        private void ThenTheVersionOfTheProjectHasBeenUpdatedTo(int version)
        {
            _retrievedProjectDetails.Version.Should().Be(version);
        }

        private void ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForEnvironment3()
        {
            ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedTo(_environment3Added.TimeStamp);
        }

        private void ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForEnvironment2()
        {
            ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedTo(_environment2Added.TimeStamp);
        }

        private void ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForToggle3()
        {
            ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedTo(_toggle3Added.TimeStamp);
        }

        private void ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForToggle2()
        {
            ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedTo(_toggle2Added.TimeStamp);
        }

        private void ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedTo(DateTimeOffset timeStamp)
        {
            _retrievedProjectDetails.LastModified.Should().Be(timeStamp);
        }
    }
}
