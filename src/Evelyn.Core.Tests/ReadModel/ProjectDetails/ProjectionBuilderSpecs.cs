namespace Evelyn.Core.Tests.ReadModel.ProjectDetails
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.ProjectDetails;
    using Core.WriteModel.Project.Domain;
    using Core.WriteModel.Project.Events;
    using Evelyn.Core.ReadModel;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectionBuilderSpecs : ReadModel.ProjectionBuilderSpecs
    {
        private readonly ProjectionBuilder _builder;

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

        private ProjectDetailsDto _dto;

        public ProjectionBuilderSpecs()
        {
            _builder = new ProjectionBuilder(StubbedRepository);
        }

        [Fact]
        public void ProjectDoesntExist()
        {
            this.Given(_ => GivenThatWeDontCreateProject1())
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject1())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void OneProjectCreated()
        {
            this.Given(_ => GivenProject1IsCreated())
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject1())
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
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject1())
                .Then(_ => ThenTheDetailsAreSetForProject1())
                .And(_ => ThenThereAreNoEnvironmentsOnTheProject())
                .And(_ => ThenThereAreNoTogglesOnTheProject())
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject2())
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
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject1())
                .Then(_ => ThenThereAreTwoEnvironmentsOnTheProject())
                .And(_ => ThenEnvrionment1IsOnTheProject())
                .And(_ => ThenEnvrionment3IsOnTheProject())
                .And(_ => ThenTheVersionOfTheProjectHasBeenUpdatedForEnvironment3())
                .And(_ => ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForEnvironment3())
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject2())
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
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject1())
                .Then(_ => ThenThereAreTwoTogglesOnTheProject())
                .And(_ => ThenToggle1IsOnTheProject())
                .And(_ => ThenToggle3IsOnTheProject())
                .And(_ => ThenTheVersionOfTheProjectHasBeenUpdatedForToggle3())
                .And(_ => ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForToggle3())
                .When(_ => WhenWeInvokeTheProjectionBuilderForProject2())
                .Then(_ => ThenThereIsOneToggleOnTheProject())
                .And(_ => ThenToggle2IsOnTheProject())
                .And(_ => ThenTheVersionOfTheProjectHasBeenUpdatedForToggle2())
                .And(_ => ThenTheLastModifiedTimeOfTheProjectHasBeenUpdatedForToggle2())
                .BDDfy();
        }

        private void GivenThatWeDontCreateProject1()
        {
            _project1Id = DataFixture.Create<Guid>();
        }

        private void GivenProject1IsCreated()
        {
            _project1Created = DataFixture.Create<ProjectCreated>();
            _project1Created.Version = StubbedRepository.NextVersionFor(_project1Created.Id);
            _project1Created.TimeStamp = DateTimeOffset.UtcNow;

            _project1Id = _project1Created.Id;

            StubbedRepository.AddEvent(_project1Created, () => new Project());
        }

        private void GivenProject2IsCreated()
        {
            _project2Created = DataFixture.Create<ProjectCreated>();
            _project2Created.Version = StubbedRepository.NextVersionFor(_project2Created.Id);
            _project2Created.TimeStamp = DateTimeOffset.UtcNow;

            _project2Id = _project2Created.Id;

            StubbedRepository.AddEvent(_project2Created, () => new Project());
        }

        private void GivenWeAddEnvironment1ToProject1()
        {
            _environment1Added = DataFixture.Create<EnvironmentAdded>();
            _environment1Added.Id = _project1Id;
            _environment1Added.Version = StubbedRepository.NextVersionFor(_project1Id);
            _environment1Added.TimeStamp = DateTimeOffset.UtcNow;

            StubbedRepository.AddEvent(_environment1Added);
        }

        private void GivenWeAddEnvironment2ToProject2()
        {
            _environment2Added = DataFixture.Create<EnvironmentAdded>();
            _environment2Added.Id = _project2Id;
            _environment2Added.Version = StubbedRepository.NextVersionFor(_project2Id);
            _environment2Added.TimeStamp = DateTimeOffset.UtcNow;

            StubbedRepository.AddEvent(_environment2Added);
        }

        private void GivenWeAddEnvironment3ToProject1()
        {
            _environment3Added = DataFixture.Create<EnvironmentAdded>();
            _environment3Added.Id = _project1Id;
            _environment3Added.Version = StubbedRepository.NextVersionFor(_project1Id);
            _environment3Added.TimeStamp = DateTimeOffset.UtcNow;

            StubbedRepository.AddEvent(_environment3Added);
        }

        private void GivenWeAddToggle1ToProject1()
        {
            _toggle1Added = DataFixture.Create<ToggleAdded>();
            _toggle1Added.Id = _project1Id;
            _toggle1Added.Version = StubbedRepository.NextVersionFor(_project1Id);
            _toggle1Added.TimeStamp = DateTimeOffset.UtcNow;

            StubbedRepository.AddEvent(_toggle1Added);
        }

        private void GivenWeAddToggle2ToProject2()
        {
            _toggle2Added = DataFixture.Create<ToggleAdded>();
            _toggle2Added.Id = _project2Id;
            _toggle2Added.Version = StubbedRepository.NextVersionFor(_project2Id);
            _toggle2Added.TimeStamp = DateTimeOffset.UtcNow;

            StubbedRepository.AddEvent(_toggle2Added);
        }

        private void GivenWeAddToggle3ToProject1()
        {
            _toggle3Added = DataFixture.Create<ToggleAdded>();
            _toggle3Added.Id = _project1Id;
            _toggle3Added.Version = StubbedRepository.NextVersionFor(_project1Id);
            _toggle3Added.TimeStamp = DateTimeOffset.UtcNow;

            StubbedRepository.AddEvent(_toggle3Added);
        }

        private async Task WhenWeInvokeTheProjectionBuilderForProject1()
        {
            await WhenWeInvokeTheProjectionBuilderFor(new ProjectionBuilderRequest(_project1Id));
        }

        private async Task WhenWeInvokeTheProjectionBuilderForProject2()
        {
            await WhenWeInvokeTheProjectionBuilderFor(new ProjectionBuilderRequest(_project2Id));
        }

        private async Task WhenWeInvokeTheProjectionBuilderFor(ProjectionBuilderRequest request)
        {
            try
            {
                _dto = await _builder.Invoke(request);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenAFailedToBuildProjectionExceptionIsThrown()
        {
            ThrownException.Should().BeOfType<FailedToBuildProjectionException>();
        }

        private void ThenThereAreNoEnvironmentsOnTheProject()
        {
            _dto.Environments.Count().Should().Be(0);
        }

        private void ThenThereAreTwoEnvironmentsOnTheProject()
        {
            _dto.Environments.Count().Should().Be(2);
        }

        private void ThenThereIsOneEnvironmentOnTheProject()
        {
            _dto.Environments.Count().Should().Be(1);
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
            _dto.Toggles.Count().Should().Be(0);
        }

        private void ThenThereAreTwoTogglesOnTheProject()
        {
            _dto.Toggles.Count().Should().Be(2);
        }

        private void ThenThereIsOneToggleOnTheProject()
        {
            _dto.Toggles.Count().Should().Be(1);
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
            _dto.Id.Should().Be(ev.Id);
            _dto.Name.Should().Be(ev.Name);
            _dto.Version.Should().Be(ev.Version);
            _dto.Created.Should().Be(ev.TimeStamp);
            _dto.LastModified.Should().Be(ev.TimeStamp);
        }

        private void ThenTheEnvironmentIsOnTheProject(EnvironmentAdded environmentAdded)
        {
            _dto.Environments.Should().Contain(environment =>
                environment.Key == environmentAdded.Key);
        }

        private void ThenTheToggleIsOnTheProject(ToggleAdded toggleAdded)
        {
            _dto.Toggles.Should().Contain(toggle =>
                toggle.Key == toggleAdded.Key &&
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
            _dto.Version.Should().Be(version);
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
            _dto.LastModified.Should().Be(timeStamp);
        }
    }
}
