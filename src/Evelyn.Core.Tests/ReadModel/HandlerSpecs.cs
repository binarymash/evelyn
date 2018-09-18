namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Core.ReadModel.EnvironmentState;
    using Core.ReadModel.ProjectDetails;
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

            AccountProjectsStore = new InMemoryProjectionStore<Guid, AccountProjectsDto>();
            ProjectDetailsStore = new InMemoryProjectionStore<Guid, ProjectDetailsDto>();
            EnvironmentDetailsStore = new InMemoryProjectionStore<string, EnvironmentDetailsDto>();
            ToggleDetailsStore = new InMemoryProjectionStore<string, ToggleDetailsDto>();
            EnvironmentStatesStore = new InMemoryProjectionStore<string, EnvironmentStateDto>();

            StubbedRepository = new StubbedRepository();

            ReadModelFacade = new DatabaseReadModelFacade(
                AccountProjectsStore,
                ProjectDetailsStore,
                EnvironmentDetailsStore,
                ToggleDetailsStore,
                EnvironmentStatesStore);

            var router = new Router();
            RegisterHandlers(router);
            _publisher = router;
        }

        protected Fixture DataFixture { get; }

        protected IReadModelFacade ReadModelFacade { get; }

        protected IProjectionStore<Guid, AccountProjectsDto> AccountProjectsStore { get; }

        protected IProjectionStore<Guid, ProjectDetailsDto> ProjectDetailsStore { get; }

        protected IProjectionStore<string, EnvironmentDetailsDto> EnvironmentDetailsStore { get; set; }

        protected IProjectionStore<string, ToggleDetailsDto> ToggleDetailsStore { get; set; }

        protected IProjectionStore<string, EnvironmentStateDto> EnvironmentStatesStore { get; set; }

        protected StubbedRepository StubbedRepository { get; set; }

        protected Exception ThrownException { get; set; }

        protected abstract void RegisterHandlers(Router router);

        protected void GivenWePublish(IEvent @event)
        {
            _publisher.Publish(@event).GetAwaiter().GetResult();
        }
    }
}
