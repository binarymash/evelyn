namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentAddedEventSpecs : EventSpecs
    {
        private EnvironmentAdded _event;

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAnEnvironmentAddedEvent())
                .Then(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .And(_ => _.GivenThereAreEnvironmentsOnTheProjection())
                .And(_ => _.GivenThereAreTogglesOnTheProjection())
                .When(_ => _.WhenWeHandleAnEnvironmentAddedEvent())
                .Then(_ => _.ThenTheProjectionIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentAddedEvent()
        {
            _event = DataFixture.Build<EnvironmentAdded>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsUpdated()
        {
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(_event.Version);

            UpdatedProjection.Toggles.Should().BeEquivalentTo(OriginalProjection.Toggles);

            var updatedEnvironments = UpdatedProjection.Environments.ToList();
            updatedEnvironments.Count.Should().Be(OriginalProjection.Environments.Count() + 1);

            foreach (var originalEnvironments in OriginalProjection.Environments)
            {
                updatedEnvironments.Should().Contain(e =>
                    e.Key == originalEnvironments.Key &&
                    e.Name == originalEnvironments.Name);
            }

            updatedEnvironments.Should().Contain(e =>
                e.Key == _event.Key &&
                e.Name == _event.Name);
        }
    }
}