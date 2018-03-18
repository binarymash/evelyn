namespace Evelyn.Core.Tests.ReadModel.EnvironmentState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.EnvironmentState;
    using Core.WriteModel.Project.Domain;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectionBuilderSpecs : ProjectionBuilderSpecs<ProjectionBuilder, EnvironmentStateDto>
    {
        private readonly List<IEvent> _projectEvents;

        private Guid _projectId;
        private Project _project;

        private string _environmentKey;
        private EnvironmentState _expectedEnvironmentState;

        public ProjectionBuilderSpecs()
        {
            Builder = new ProjectionBuilder(SubstituteRepository);
            _projectEvents = new List<IEvent>();
        }

        [Fact]
        public void ProjectDoesntExist()
        {
            this.Given(_ => GivenTheProjectDoesNotExistInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilderForTheEnvironmentState())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveAProjectButItDoesntHaveOurEnvironment())
                .And(_ => GivenTheProjectIsInTheRepository())
                .And(_ => GivenTheEnvironmentDoesNotExistInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilderForTheEnvironmentState())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentExists()
        {
            this.Given(_ => GivenWeHaveAProjectWithEnvironmentsAndToggles())
                .And(_ => GivenTheProjectIsInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilderForTheEnvironmentState())
                .Then(_ => ThenTheEnvironmentVersionIsSet())
                .And(_ => ThenAllTheToggleStatesAreSet())
                .BDDfy();
        }

        private void GivenTheProjectDoesNotExistInTheRepository()
        {
            _projectId = DataFixture.Create<Guid>();

            SubstituteRepository
                .Get<Project>(_projectId, Arg.Any<CancellationToken>())
                .Throws(new AggregateNotFoundException(typeof(Project), _projectId));
        }

        private void GivenTheProjectIsInTheRepository()
        {
            SubstituteRepository
                .Get<Project>(_projectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(_project));
        }

        private void GivenWeHaveAProjectButItDoesntHaveOurEnvironment()
        {
            _projectId = DataFixture.Create<Guid>();
            _environmentKey = DataFixture.Create<string>();

            _projectEvents.Add(DataFixture.Build<ProjectCreated>()
                .With(ev => ev.Version, 0)
                .With(ev => ev.Id, _projectId)
                .Create());

            _project = new Project();
            _project.LoadFromHistory(_projectEvents);
        }

        private void GivenWeHaveAProjectWithEnvironmentsAndToggles()
        {
            _projectId = DataFixture.Create<Guid>();
            _environmentKey = DataFixture.Create<string>();

            _projectEvents.Add(DataFixture.Build<ProjectCreated>()
                .With(ev => ev.Version, 0)
                .With(ev => ev.Id, _projectId)
                .Create());

            _projectEvents.Add(DataFixture.Build<EnvironmentAdded>()
                .With(ev => ev.Version, 1)
                .With(ev => ev.Id, _projectId)
                .With(ev => ev.Key, _environmentKey)
                .Create());

            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
                .With(ev => ev.Version, 2)
                .With(ev => ev.Id, _projectId)
                .Create());

            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
                .With(ev => ev.Version, 3)
                .With(ev => ev.Id, _projectId)
                .Create());

            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
                .With(ev => ev.Version, 4)
                .With(ev => ev.Id, _projectId)
                .Create());

            _project = new Project();
            _project.LoadFromHistory(_projectEvents);

            _expectedEnvironmentState = _project.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
        }

        private void GivenTheEnvironmentDoesNotExistInTheRepository()
        {
            _environmentKey = DataFixture.Create<string>();
        }

        private async Task WhenWeInvokeTheProjectionBuilderForTheEnvironmentState()
        {
            await WhenWeInvokeTheProjectionBuilderFor(new ProjectionBuilderRequest(_projectId, _environmentKey));
        }

        private async Task WhenWeInvokeTheProjectionBuilderFor(ProjectionBuilderRequest request)
        {
            try
            {
                Dto = await Builder.Invoke(request);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenTheEnvironmentVersionIsSet()
        {
            Dto.Version.Should().Be(_expectedEnvironmentState.Version);
        }

        private void ThenAllTheToggleStatesAreSet()
        {
            Dto.ToggleStates.Count().Should().Be(_expectedEnvironmentState.ToggleStates.Count());

            foreach (var toggleState in _expectedEnvironmentState.ToggleStates)
            {
                Dto.ToggleStates.ToList().Exists(MatchingToggleState(toggleState)).Should().BeTrue();
            }
        }

        private Predicate<ToggleStateDto> MatchingToggleState(ToggleState toggleState)
        {
            return ts =>
                ts.Key == toggleState.Key &&
                ts.Value == toggleState.Value;
        }
    }
}
