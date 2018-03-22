namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Core.WriteModel.Project;
    using Core.WriteModel.Project.Domain;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Commands;
    using FluentAssertions;

    public abstract class ProjectCommandHandlerSpecs<TCommand> : CommandHandlerSpecs<Project, ProjectCommandHandler, TCommand>
        where TCommand : ICommand
    {
        protected override ProjectCommandHandler BuildHandler()
        {
            return new ProjectCommandHandler(Session);
        }

        protected void GivenWeHaveCreatedAProjectWith(Guid id)
        {
            var projectCreated = DataFixture.Create<ProjectCreated>();
            projectCreated.Id = id;
            projectCreated.Version = HistoricalEvents.Count;

            HistoricalEvents.Add(projectCreated);
        }

        protected void GivenWeHaveAddedAnEnvironmentWith(Guid projectId, string environmentKey)
        {
            var environmentAdded = DataFixture.Create<EnvironmentAdded>();
            environmentAdded.Id = projectId;
            environmentAdded.Key = environmentKey;
            environmentAdded.Version = HistoricalEvents.Count;

            HistoricalEvents.Add(environmentAdded);
        }

        protected void GivenWeHaveAddedAnEnvironmentStateWith(Guid projectId, string environmentKey, IEnumerable<KeyValuePair<string, string>> toggleStates = null)
        {
            var environmentStateAdded = new EnvironmentStateAdded(
                DataFixture.Create<string>(),
                projectId,
                environmentKey,
                DateTime.UtcNow,
                toggleStates)
            {
                Version = HistoricalEvents.Count
            };

            HistoricalEvents.Add(environmentStateAdded);
        }

        protected void ThenTheAggregateRootScopedVersionHasBeenIncreasedBy(int increment)
        {
            NewAggregate.ScopedVersion.Should().Be(OriginalAggregate.ScopedVersion + increment);
        }
    }
}
