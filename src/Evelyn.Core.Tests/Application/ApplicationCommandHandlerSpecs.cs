namespace Evelyn.Core.Tests.Application
{
    using CQRSlite.Commands;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Domain;
    using Evelyn.Core.WriteModel.Handlers;
    using System;

    public abstract class ApplicationCommandHandlerSpecs<TCommand> : CommandHandlerSpecs<Application, ApplicationCommandHandler, TCommand>
        where TCommand : ICommand
    {
        public ApplicationCommandHandlerSpecs()
        {
        }

        protected override ApplicationCommandHandler BuildHandler()
        {
            return new ApplicationCommandHandler(Session);
        }

        protected void GivenWeHaveCreatedAnApplicationWith(Guid id)
        {
            var name = "my application name";
            HistoricalEvents.Add(new ApplicationCreated(id, name) { Version = HistoricalEvents.Count + 1 });
        }

        protected void GivenWeHaveAddedAnEnvironmentWith(Guid applicationId, Guid environmentId)
        {
            var name = "my environment name";
            var key = "my environment key";
            HistoricalEvents.Add(new EnvironmentAdded(applicationId, environmentId, name, key) { Version = HistoricalEvents.Count + 1 });
        }
    }
}
