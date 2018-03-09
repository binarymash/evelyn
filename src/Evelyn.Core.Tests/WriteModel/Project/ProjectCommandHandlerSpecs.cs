namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using AutoFixture;
    using CQRSlite.Commands;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Project.Domain;
    using Evelyn.Core.WriteModel.Project.Handlers;

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

        protected void GivenWeHaveAddedAnEnvironmentWith(Guid projectedId, string environmentKey)
        {
            var environmentAdded = DataFixture.Create<EnvironmentAdded>();
            environmentAdded.Id = projectedId;
            environmentAdded.Key = environmentKey;
            environmentAdded.Version = HistoricalEvents.Count;

            HistoricalEvents.Add(environmentAdded);
        }
    }
}
