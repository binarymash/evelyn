namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.AccountEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectCreatedSpecs : EventSpecs<ProjectCreated>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .And(_ => ThenNoProjectionIsCreated())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsNotOnTheProjection())
                .And(_ => GivenAnotherProjectIsOnTheProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectHadBeenAddedToTheProjection())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            Event = DataFixture.Build<ProjectCreated>()
                .With(pc => pc.Id, AccountId)
                .With(pc => pc.ProjectId, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectHadBeenAddedToTheProjection()
        {
            UpdatedProjection.AccountId.Should().Be(OriginalProjection.AccountId);
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.Version.Should().Be(Event.Version);

            var projects = UpdatedProjection.Projects.ToList();

            projects.Count.Should().Be(OriginalProjection.Projects.Count() + 1);

            foreach (var originalProject in OriginalProjection.Projects)
            {
                projects.Exists(p =>
                    p.Id == Event.ProjectId &&
                    p.Name == string.Empty).Should().BeTrue();
            }

            projects.Exists(p =>
                p.Id == Event.ProjectId &&
                p.Name == string.Empty).Should().BeTrue();
        }
    }
}
