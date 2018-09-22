namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;
    using Core.ReadModel.Projections.AccountProjects;
    using Core.ReadModel.Projections.EnvironmentDetails;
    using Core.ReadModel.Projections.EnvironmentState;
    using Core.ReadModel.Projections.ProjectDetails;
    using Core.ReadModel.Projections.ToggleDetails;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Infrastructure;

    public abstract class HandlerSpecs
    {
        private readonly IEventPublisher _publisher;

        protected HandlerSpecs()
        {
            DataFixture = new Fixture();

            AccountProjectsStore = new InMemoryProjectionStore<AccountProjectsDto>();
            ProjectDetailsStore = new InMemoryProjectionStore<ProjectDetailsDto>();
            EnvironmentDetailsStore = new InMemoryProjectionStore<EnvironmentDetailsDto>();
            ToggleDetailsStore = new InMemoryProjectionStore<ToggleDetailsDto>();
            EnvironmentStatesStore = new InMemoryProjectionStore<EnvironmentStateDto>();

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

        protected IProjectionStore<AccountProjectsDto> AccountProjectsStore { get; }

        protected IProjectionStore<ProjectDetailsDto> ProjectDetailsStore { get; }

        protected IProjectionStore<EnvironmentDetailsDto> EnvironmentDetailsStore { get; set; }

        protected IProjectionStore<ToggleDetailsDto> ToggleDetailsStore { get; set; }

        protected IProjectionStore<EnvironmentStateDto> EnvironmentStatesStore { get; set; }

        protected StubbedRepository StubbedRepository { get; set; }

        protected Exception ThrownException { get; set; }

        protected abstract void RegisterHandlers(Router router);

        protected void GivenWePublish(IEvent @event)
        {
            _publisher.Publish(@event).GetAwaiter().GetResult();
        }
    }
}
