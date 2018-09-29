namespace Evelyn.Core.Tests.ReadModel.AccountProjects.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectCreatedSpecs
    {
        private Fixture _fixture;
        private IProjectionStore<AccountProjectsDto> _projectionStore;
        private ProjectEvents.ProjectCreated @event;
        private ProjectionBuilder _projectionBuilder;
        private CancellationToken _stoppingToken;
        private Guid _accountId;
        private Guid _projectId;
        private AccountProjectsDto _originalProjection;
        private Exception _thrownException;
        private AccountProjectsDto _updatedProjection;

        public ProjectCreatedSpecs()
        {
            _fixture = new Fixture();
            _projectionStore = new InMemoryProjectionStore<AccountProjectsDto>();
            _stoppingToken = default;
        }

        [Fact]
        public void NoProjection()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectNotOnProjection()
        {
            this.Given(_ => GivenThereIsAProjection())
                .And(_ => GivenTheProjectIsNotOnTheProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectIsOnProjection()
        {
            this.Given(_ => GivenThereIsAProjection())
                .And(_ => GivenTheProjectIsOnTheProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        private void GivenThereIsNoProjection()
        {
            _accountId = _fixture.Create<Guid>();
            _originalProjection = null;
        }

        private void GivenThereIsAProjection()
        {
            _accountId = _fixture.Create<Guid>();

            _originalProjection = AccountProjectsDto.Create(
                _accountId,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());
        }

        private void GivenTheProjectIsNotOnTheProjection()
        {
            _projectId = _fixture.Create<Guid>();
        }

        private void GivenTheProjectIsOnTheProjection()
        {
            _projectId = _fixture.Create<Guid>();

            _originalProjection.AddProject(
                _projectId,
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            @event = _fixture.Build<ProjectEvents.ProjectCreated>()
                .With(e => e.AccountId, _accountId)
                .With(e => e.Id, _projectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private async Task WhenTheEventIsHandled()
        {
            if (_originalProjection != null)
            {
                await _projectionStore.Create(AccountProjectsDto.StoreKey(_originalProjection.AccountId), _originalProjection);
            }

            _projectionBuilder = new ProjectionBuilder(_projectionStore);

            try
            {
                await _projectionBuilder.Handle(@event, _stoppingToken);
                _updatedProjection = await _projectionStore.Get(AccountProjectsDto.StoreKey(@event.AccountId));
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void ThenAnExceptionIsThrown()
        {
            _thrownException.Should().NotBeNull();
        }

        private void ThenTheProjectionHasBeenUpdated()
        {
            _updatedProjection.AccountId.Should().Be(_originalProjection.AccountId);
            _updatedProjection.Created.Should().Be(_originalProjection.Created);
            _updatedProjection.CreatedBy.Should().Be(_originalProjection.CreatedBy);

            _updatedProjection.LastModified.Should().Be(_originalProjection.LastModified);
            _updatedProjection.LastModifiedBy.Should().Be(_originalProjection.LastModifiedBy);
            _updatedProjection.Version.Should().Be(_originalProjection.Version);

            var projects = _updatedProjection.Projects.ToList();

            projects.Count.Should().Be(_originalProjection.Projects.Count());

            foreach (var originalProject in _originalProjection.Projects.Where(p => p.Id != @event.Id))
            {
                projects.Exists(p =>
                    p.Id == originalProject.Id &&
                    p.Name == originalProject.Name).Should().BeTrue();
            }

            projects.Exists(p =>
                p.Id == @event.Id &&
                p.Name == @event.Name).Should().BeTrue();
        }
    }
}
