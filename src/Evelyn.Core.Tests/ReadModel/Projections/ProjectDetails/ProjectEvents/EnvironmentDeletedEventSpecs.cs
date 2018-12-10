namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentDeletedEventSpecs : ProjectionBuilderHarness<EnvironmentDeleted>
    {
        private string _environmentKey;

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnEnvironmentDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurEnvironmentIsOnTheProjection())
                .And(_ => GivenThereAreEnvironmentsOnTheProject())
                .When(_ => WhenWeHandleAnEnvironmentDeletedEvent())
                .Then(_ => ThenOurEnvironmentIsDeleted())
                .And(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheProjectAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private void GivenOurEnvironmentIsOnTheProjection()
        {
            _environmentKey = DataFixture.Create<string>();

            OriginalProjection.Project.AddEnvironment(
                DataFixture.Create<EventAuditDto>(),
                _environmentKey,
                DataFixture.Create<string>());
        }

        private async Task WhenWeHandleAnEnvironmentDeletedEvent()
        {
            Event = DataFixture.Build<EnvironmentDeleted>()
                .With(ar => ar.Id, ProjectId)
                .With(ar => ar.Key, _environmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurEnvironmentIsDeleted()
        {
            UpdatedProjection.Project.Toggles.Should().BeEquivalentTo(OriginalProjection.Project.Toggles);

            var updatedEnvironments = UpdatedProjection.Project.Environments.ToList();
            updatedEnvironments.Count.Should().Be(OriginalProjection.Project.Environments.Count() - 1);

            foreach (var originalEnvironment in OriginalProjection.Project.Environments)
            {
                if (originalEnvironment.Key == Event.Key)
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