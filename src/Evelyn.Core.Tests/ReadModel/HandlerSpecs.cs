namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Core.ReadModel.ProjectDetails;
    using Core.ReadModel.ProjectList;
    using Core.ReadModel.ToggleDetails;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Infrastructure;

    public abstract class HandlerSpecs
    {
        private readonly IEventPublisher _publisher;

        protected HandlerSpecs()
        {
            DataFixture = new Fixture();

            AccountProjectsStore = new InMemoryDatabase<string, AccountProjectsDto>();
            ProjectDetailsStore = new InMemoryDatabase<Guid, ProjectDetailsDto>();
            EnvironmentDetailsStore = new InMemoryDatabase<string, EnvironmentDetailsDto>();
            ToggleDetailsStore = new InMemoryDatabase<Guid, ToggleDetailsDto>();

            ReadModelFacade = new DatabaseReadModelFacade(
                AccountProjectsStore,
                ProjectDetailsStore,
                EnvironmentDetailsStore,
                ToggleDetailsStore);

            var router = new Router();
            RegisterHandlers(router);
            _publisher = router;
        }

        protected Fixture DataFixture { get; }

        protected IReadModelFacade ReadModelFacade { get; }

        protected IDatabase<string, AccountProjectsDto> AccountProjectsStore { get; }

        protected IDatabase<Guid, ProjectDetailsDto> ProjectDetailsStore { get; }

        protected IDatabase<string, EnvironmentDetailsDto> EnvironmentDetailsStore { get; set; }

        protected IDatabase<Guid, ToggleDetailsDto> ToggleDetailsStore { get; set; }

        protected Exception ThrownException { get; set; }

        protected abstract void RegisterHandlers(Router router);

        protected void GivenWePublish(IEvent @event)
        {
            _publisher.Publish(@event).GetAwaiter().GetResult();
        }
    }
}
