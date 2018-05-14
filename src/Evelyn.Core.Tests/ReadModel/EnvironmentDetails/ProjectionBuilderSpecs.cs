namespace Evelyn.Core.Tests.ReadModel.EnvironmentDetails
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
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;
    using Environment = Core.WriteModel.Project.Domain.Environment;

    public class ProjectionBuilderSpecs : ProjectionBuilderSpecs<ProjectionBuilder, EnvironmentDetailsDto>
    {
        private readonly List<IEvent> _projectEvents;

        private Guid _projectId;
        private Project _project;

        private string _environmentKey;
        private Environment _expectedEnvironment;

        public ProjectionBuilderSpecs()
        {
            _projectEvents = new List<IEvent>();
            Builder = new ProjectionBuilder(SubstituteRepository);
        }

        [Fact]
        public void ProjectDoesntExist()
        {
            this.Given(_ => GivenTheProjectDoesNotExistInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenANullProjectionIsReturned())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentDoesNotExist()
        {
            this.Given(_ => GivenWeHaveAProjectButItDoesntHaveOurEnvironment())
                .And(_ => GivenTheProjectIsInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenANullProjectionIsReturned())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentExists()
        {
            this.Given(_ => GivenWeHaveAProjectWithEnvironments())
                .And(_ => GivenTheProjectIsInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenTheVersionIsSet())
                .Then(_ => ThenTheCreatedDateIsSet())
                .And(_ => ThenTheCreatedByIsSet())
                .And(_ => ThenTheLastModifiedDateIsSet())
                .And(_ => ThenTheLastModifiedByIsSet())
                .And(_ => ThenTheProjectIdIsSet())
                .And(_ => ThenTheEnvironmentKeyIsSet())
                .BDDfy();
        }

        [Fact]
        public void SomeOtherExceptionFromEventStore()
        {
            this.Given(_ => GivenTheEventStoreThrowsSomeOtherException())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
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

        private void GivenWeHaveAProjectWithEnvironments()
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

            _project = new Project();
            _project.LoadFromHistory(_projectEvents);

            _expectedEnvironment = _project.Environments.First(es => es.Key == _environmentKey);
        }

        private void GivenTheEventStoreThrowsSomeOtherException()
        {
            SubstituteRepository
                .Get<Project>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Throws(DataFixture.Create<Exception>());
        }

        private async Task WhenWeInvokeTheProjectionBuilder()
        {
            try
            {
                var request = new ProjectionBuilderRequest(_projectId, _environmentKey);
                Dto = await Builder.Invoke(request);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenTheVersionIsSet()
        {
            Dto.Version.Should().Be(_expectedEnvironment.ScopedVersion);
        }

        private void ThenTheCreatedDateIsSet()
        {
            Dto.Created.Should().Be(_expectedEnvironment.Created);
        }

        private void ThenTheCreatedByIsSet()
        {
            Dto.CreatedBy.Should().Be(_expectedEnvironment.CreatedBy);
        }

        private void ThenTheLastModifiedDateIsSet()
        {
            Dto.LastModified.Should().Be(_expectedEnvironment.LastModified);
        }

        private void ThenTheLastModifiedByIsSet()
        {
            Dto.LastModifiedBy.Should().Be(_expectedEnvironment.LastModifiedBy);
        }

        private void ThenTheProjectIdIsSet()
        {
            Dto.ProjectId.Should().Be(_projectId);
        }

        private void ThenTheEnvironmentKeyIsSet()
        {
            Dto.Key.Should().Be(_expectedEnvironment.Key);
        }
    }
}
