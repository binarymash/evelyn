namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Core.WriteModel.Project.Domain;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Commands;

    public abstract class ProjectCommandHandlerSpecs<THandler, TCommand> : CommandHandlerSpecs<Project, THandler, TCommand>
        where THandler : class
        where TCommand : ICommand
    {
        protected void GivenWeHaveCreatedAProjectWith(Guid id)
        {
            GivenWeHaveCreatedAProjectWith(id, HistoricalEvents.Count);
        }

        protected void GivenWeHaveCreatedAProjectWith(Guid id, int version)
        {
            var projectCreated = DataFixture.Create<ProjectCreated>();
            projectCreated.Id = id;
            projectCreated.Version = version;

            HistoricalEvents.Add(projectCreated);
        }

        protected void GivenWeHaveAddedAnEnvironmentWith(Guid projectId, string environmentKey)
        {
            GivenWeHaveAddedAnEnvironmentWith(projectId, environmentKey, HistoricalEvents.Count);
        }

        protected void GivenWeHaveAddedAnEnvironmentWith(Guid projectId, string environmentKey, int version)
        {
            var environmentAdded = DataFixture.Create<EnvironmentAdded>();
            environmentAdded.Id = projectId;
            environmentAdded.Key = environmentKey;
            environmentAdded.Version = version;

            HistoricalEvents.Add(environmentAdded);
        }

        protected void GivenWeHaveAddedAnEnvironmentStateWith(Guid projectId, string environmentKey, IEnumerable<KeyValuePair<string, string>> toggleStates = null)
        {
            GivenWeHaveAddedAnEnvironmentStateWith(projectId, environmentKey, HistoricalEvents.Count, toggleStates);
        }

        protected void GivenWeHaveAddedAnEnvironmentStateWith(Guid projectId, string environmentKey, int version, IEnumerable<KeyValuePair<string, string>> toggleStates = null)
        {
            var environmentStateAdded = new EnvironmentStateAdded(
                DataFixture.Create<string>(),
                projectId,
                environmentKey,
                DateTime.UtcNow,
                toggleStates)
            {
                Version = version
            };

            HistoricalEvents.Add(environmentStateAdded);
        }

        protected void ThenAProjectDeletedExceptionIsThrownFor(Guid id)
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"The project with id {id} has already been deleted");
        }
    }
}
