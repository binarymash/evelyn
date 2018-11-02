namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentDeletedEventSpecs : EventSpecs
    {
        private EnvironmentDeleted _event;

        public string EnvironmentKey { get; private set; }

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAnEnvironmentDeletedEvent())
                .Then(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .And(_ => _.GivenOurEnvironmentIsOnTheProjection())
                .And(_ => _.GivenThereAreEnvironmentsOnTheProjection())
                .When(_ => _.WhenWeHandleAnEnvironmentDeletedEvent())
                .Then(_ => _.ThenTheProjectionIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private void GivenOurEnvironmentIsOnTheProjection()
        {
            EnvironmentKey = DataFixture.Create<string>();

            OriginalProjection.AddEnvironment(
                EnvironmentKey,
                DataFixture.Create<string>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<int>(),
                DataFixture.Create<string>());
        }

        private async Task WhenWeHandleAnEnvironmentDeletedEvent()
        {
            _event = DataFixture.Build<EnvironmentDeleted>()
                .With(ar => ar.Id, ProjectId)
                .With(ar => ar.Key, EnvironmentKey)
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
            updatedEnvironments.Count.Should().Be(OriginalProjection.Environments.Count() - 1);

            foreach (var originalEnvironment in OriginalProjection.Environments)
            {
                if (originalEnvironment.Key == _event.Key)
                {
                    continue;
                }

                updatedEnvironments.Should().Contain(e =>
                    e.Key == originalEnvironment.Key &&
                    e.Name == originalEnvironment.Name);
            }
        }
    }
}