namespace Evelyn.Core.Tests.WriteModel.Account
{
    using System;
    using AutoFixture;
    using Core.WriteModel.Account;
    using Core.WriteModel.Account.Domain;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Commands;

    public abstract class AccountCommandHandlerSpecs<TCommand> : CommandHandlerSpecs<Account, AccountCommandHandler, TCommand>
        where TCommand : ICommand
    {
        protected override AccountCommandHandler BuildHandler()
        {
            return new AccountCommandHandler(Session);
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
