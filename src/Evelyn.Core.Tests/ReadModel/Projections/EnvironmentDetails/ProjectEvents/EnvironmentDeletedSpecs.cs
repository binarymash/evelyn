namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class EnvironmentDeletedSpecs : EventSpecs
    {
        private ProjectEvents.EnvironmentDeleted _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .When(_ => WhenWeHandleAnEnvironmentDeletedEvent())
                .Then(_ => ThenTheProjectionIsDeleted())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentDeletedEvent()
        {
            _event = DataFixture.Build<ProjectEvents.EnvironmentDeleted>()
                .With(e => e.Id, ProjectId)
                .With(e => e.Key, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsDeleted()
        {
            UpdatedProjection.Should().BeNull();
        }
    }
}
