namespace Evelyn.Core.Tests.WriteModel.Account.CreateProject
{
    using System;
    using AutoFixture;
    using Core.WriteModel.Account.Commands.CreateProject;
    using Core.WriteModel.Account.Domain;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Commands;

    public abstract class HandlerSpecs<TCommand> : CommandHandlerSpecs<Account, Handler, TCommand>
        where TCommand : ICommand
    {
        protected override Handler BuildHandler()
        {
            return new Handler(Session);
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
