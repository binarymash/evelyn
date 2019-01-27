namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState.ProjectEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public class EnvironmentStateAddedSpecs : ProjectionBuilderHarness<EnvironmentStateAdded>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereAreNoProjections())
                .When(_ => WhenWeHandleAnEnvironmentStateAddedEvent())
                .Then(_ => ThenAProjectionIsCreatedWithTheNewEnvironmentState())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectAlreadyHasAToggleState())
                .When(_ => WhenWeHandleAnEnvironmentStateAddedEvent())
                .Then(_ => ThenTheNewToggleStateIsAdded())
                .And(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        [Fact]
        public void ProjectionAlreadyBuilt()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectAlreadyHasAToggleState())
                .And(_ => GivenTheProjectionStreamVersionIsTheSameAsTheNextEvent())
                .When(_ => WhenWeHandleAnEnvironmentStateAddedEvent())
                .Then(_ => ThenTheStoredProjectionIsUnchanged())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private void GivenTheProjectAlreadyHasAToggleState()
        {
            OriginalProjection.ToggleState.AddEnvironmentState(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        private void GivenTheProjectionStreamVersionIsTheSameAsTheNextEvent()
        {
            StreamPosition = OriginalProjection.Audit.StreamPosition;
        }

        private async Task WhenWeHandleAnEnvironmentStateAddedEvent()
        {
            Event = new EnvironmentStateAdded(
                DataFixture.Create<string>(),
                ProjectId,
                EnvironmentKey,
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.CreateMany<KeyValuePair<string, string>>(1));

            await WhenTheEventIsHandled();
        }

        private void ThenAProjectionIsCreatedWithTheNewEnvironmentState()
        {
            var environmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            environmentStates.Count.Should().Be(1);

            environmentStates.Should().Contain(ts =>
                ts.Key == Event.EnvironmentKey &&
                ts.Value == Event.ToggleStates.ToList()[0].Value);
        }

        private void ThenTheNewToggleStateIsAdded()
        {
            var environmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            environmentStates.Count.Should().Be(OriginalProjection.ToggleState.EnvironmentStates.Count() + 1);

            foreach (var environmentState in OriginalProjection.ToggleState.EnvironmentStates)
            {
                environmentStates.Should().Contain(ts =>
                    ts.Key == environmentState.Key &&
                    ts.Value == environmentState.Value);
            }

            environmentStates.Should().Contain(ts =>
                ts.Key == Event.EnvironmentKey &&
                ts.Value == Event.ToggleStates.ToList()[0].Value);
        }
    }
}
