namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectCreatedSpecs : EventSpecs
    {
        private ProjectEvents.ProjectCreated _event;

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
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsNotOnTheProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectIsOnProjection()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsOnTheProjection())
                .And(_ => GivenAnotherProjectIsOnTheProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenOurProjectNameHasBeenUpdated())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByProjectionStoreWhenSaving()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsOnTheProjection())
                .And(_ => GivenTheProjectionStoreWillThrowWhenUpdating())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            _event = DataFixture.Build<ProjectEvents.ProjectCreated>()
                .With(e => e.AccountId, AccountId)
                .With(e => e.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurProjectNameHasBeenUpdated()
        {
            UpdatedProjection.AccountId.Should().Be(OriginalProjection.AccountId);
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(OriginalProjection.LastModified);
            UpdatedProjection.LastModifiedBy.Should().Be(OriginalProjection.LastModifiedBy);
            UpdatedProjection.Version.Should().Be(OriginalProjection.Version);

            var projects = UpdatedProjection.Projects.ToList();

            projects.Count.Should().Be(OriginalProjection.Projects.Count());

            foreach (var originalProject in OriginalProjection.Projects.Where(p => p.Id != _event.Id))
            {
                projects.Exists(p =>
                    p.Id == originalProject.Id &&
                    p.Name == originalProject.Name).Should().BeTrue();
            }

            projects.Exists(p =>
                p.Id == _event.Id &&
                p.Name == _event.Name).Should().BeTrue();
        }
    }
}
