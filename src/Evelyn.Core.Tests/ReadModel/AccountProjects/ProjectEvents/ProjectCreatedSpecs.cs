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
            this.Given(_ => GivenAProjectCreatedEvent())
                .And(_ => GivenThereIsNoProjectionForTheAccountProjects())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectNotOnProjection()
        {
            this.Given(_ => GivenAProjectCreatedEvent())
                .And(_ => GivenThereIsAProjectionForTheAccountProjects())
                .And(_ => GivenTheProjectIsNotOnTheProjection())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectIsOnProjection()
        {
            this.Given(_ => GivenAProjectCreatedEvent())
                .And(_ => GivenThereIsAProjectionForTheAccountProjects())
                .And(_ => GivenTheProjectIsOnTheProjection())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        private void GivenAProjectCreatedEvent()
        {
            @event = _fixture.Create<ProjectEvents.ProjectCreated>();
        }

        private void GivenThereIsNoProjectionForTheAccountProjects()
        {
            _originalProjection = null;
        }

        private void GivenThereIsAProjectionForTheAccountProjects()
        {
            _originalProjection = AccountProjectsDto.Create(
                @event.AccountId,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());
        }

        private void GivenTheProjectIsNotOnTheProjection()
        {
        }

        private void GivenTheProjectIsOnTheProjection()
        {
            _originalProjection.AddProject(
                @event.Id,
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());
        }

        private async Task WhenTheEventIsHandled()
        {
            if (_originalProjection != null)
            {
                await _projectionStore.AddOrUpdate(AccountProjectsDto.StoreKey(_originalProjection.AccountId), _originalProjection);
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
