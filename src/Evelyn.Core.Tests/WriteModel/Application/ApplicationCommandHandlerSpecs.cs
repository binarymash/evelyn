namespace Evelyn.Core.Tests.WriteModel.Application
{
    using System;
    using AutoFixture;
    using CQRSlite.Commands;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Domain;
    using Evelyn.Core.WriteModel.Handlers;

    public abstract class ApplicationCommandHandlerSpecs<TCommand> : CommandHandlerSpecs<Application, ApplicationCommandHandler, TCommand>
        where TCommand : ICommand
    {
        protected override ApplicationCommandHandler BuildHandler()
        {
            return new ApplicationCommandHandler(Session);
        }

        protected void GivenWeHaveCreatedAnApplicationWith(Guid id)
        {
            var applicationCreated = DataFixture.Create<ApplicationCreated>();
            applicationCreated.Id = id;
            applicationCreated.Version = HistoricalEvents.Count + 1;

            HistoricalEvents.Add(applicationCreated);
        }

        protected void GivenWeHaveAddedAnEnvironmentWith(Guid applicationId, Guid environmentId)
        {
            var environmentAdded = DataFixture.Create<EnvironmentAdded>();
            environmentAdded.Id = applicationId;
            environmentAdded.EnvironmentId = environmentId;
            environmentAdded.Version = HistoricalEvents.Count + 1;

            HistoricalEvents.Add(environmentAdded);
        }
    }
}
