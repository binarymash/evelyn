namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState.ProjectEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ToggleState;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentStateDeletedSpecs : ProjectionBuilderHarness<EnvironmentStateDeleted>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereAreNoProjections())
                .When(_ => WhenWeHandleAnEnvironmentStateDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void MultipleToggleStatesExist()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectionHasStateForOurEnvironment())
                .And(_ => GivenTheProjectionHasStateForAnotherEnvironment())
                .When(_ => WhenWeHandleAnEnvironmentStateDeletedEvent())
                .Then(_ => ThenTheEnvironmentStateIsRemoved())
                .And(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        [Fact]
        public void LastToggleStateIsDeleted()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectionHasStateForOurEnvironment())
                .When(_ => WhenWeHandleAnEnvironmentStateDeletedEvent())
                .Then(_ => ThenTheProjectionIsDeleted())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentStateDeletedEvent()
        {
            Event = new EnvironmentStateDeleted(
                DataFixture.Create<string>(),
                ProjectId,
                EnvironmentKey,
                DataFixture.Create<DateTimeOffset>(),
                new List<string>() { ToggleKey });

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsDeleted()
        {
            ProjectionStore.Received().Delete(Projection.StoreKey(ProjectId, ToggleKey));
        }

        private void ThenTheEnvironmentStateIsRemoved()
        {
            var environmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            environmentStates.Count.Should().Be(OriginalProjection.ToggleState.EnvironmentStates.Count() - 1);

            foreach (var environmentState in OriginalProjection.ToggleState.EnvironmentStates)
            {
                if (environmentState.Key != Event.EnvironmentKey)
                {
                    environmentStates.Should().Contain(ts =>
                        ts.Key == environmentState.Key &&
                        ts.Value == environmentState.Value);
                }
            }
        }
    }
}
