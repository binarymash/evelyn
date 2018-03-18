namespace Evelyn.Core.Tests.ReadModel.ToggleDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.WriteModel.Project.Domain;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectionBuilderSpecs : ReadModel.ProjectionBuilderSpecs
    {
        private readonly ProjectionBuilder _builder;
        private readonly List<IEvent> _projectEvents;

        private Guid _projectId;
        private Project _project;

        private string _toggleKey;
        private Toggle _expectedToggle;

        private ToggleDetailsDto _dto;

        public ProjectionBuilderSpecs()
        {
            _projectEvents = new List<IEvent>();
            _builder = new ProjectionBuilder(SubstituteRepository);
        }

        [Fact]
        public void ProjectDoesntExist()
        {
            this.Given(_ => GivenTheProjectDoesNotExistInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ToggleDoesNotExist()
        {
            this.Given(_ => GivenWeHaveAProjectButItDoesntHaveOurToggle())
                .And(_ => GivenTheProjectIsInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ToggleExists()
        {
            this.Given(_ => GivenWeHaveAProjectWithToggles())
                .And(_ => GivenTheProjectIsInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenTheCreatedDateIsSet())
                .And(_ => ThenTheLastModifiedDateIsSet())
                .And(_ => ThenTheProjectIdIsSet())
                .And(_ => ThenTheToggleKeyIsSet())
                .And(_ => ThenTheToggleNameIsSet())
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

        private void GivenWeHaveAProjectButItDoesntHaveOurToggle()
        {
            _projectId = DataFixture.Create<Guid>();
            _toggleKey = DataFixture.Create<string>();

            _projectEvents.Add(DataFixture.Build<ProjectCreated>()
                .With(ev => ev.Version, 0)
                .With(ev => ev.Id, _projectId)
                .Create());

            _project = new Project();
            _project.LoadFromHistory(_projectEvents);
        }

        private void GivenWeHaveAProjectWithToggles()
        {
            _projectId = DataFixture.Create<Guid>();
            _toggleKey = DataFixture.Create<string>();

            _projectEvents.Add(DataFixture.Build<ProjectCreated>()
                .With(ev => ev.Version, 0)
                .With(ev => ev.Id, _projectId)
                .Create());

            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
                .With(ev => ev.Version, 1)
                .With(ev => ev.Id, _projectId)
                .With(ev => ev.Key, _toggleKey)
                .Create());

            _project = new Project();
            _project.LoadFromHistory(_projectEvents);

            _expectedToggle = _project.Toggles.First(es => es.Key == _toggleKey);
        }

        private async Task WhenWeInvokeTheProjectionBuilder()
        {
            try
            {
                var request = new ProjectionBuilderRequest(_projectId, _toggleKey);
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

        private void ThenTheCreatedDateIsSet()
        {
            _dto.Created.Should().Be(_expectedToggle.Created);
        }

        private void ThenTheLastModifiedDateIsSet()
        {
            _dto.LastModified.Should().Be(_expectedToggle.LastModified);
        }

        private void ThenTheProjectIdIsSet()
        {
            _dto.ProjectId.Should().Be(_projectId);
        }

        private void ThenTheToggleKeyIsSet()
        {
            _dto.Key.Should().Be(_expectedToggle.Key);
        }

        private void ThenTheToggleNameIsSet()
        {
            _dto.Name.Should().Be(_expectedToggle.Name);
        }
    }
}
