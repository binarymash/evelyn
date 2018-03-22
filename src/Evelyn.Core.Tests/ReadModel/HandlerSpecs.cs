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

            AccountProjectsStore = new InMemoryDatabase<Guid, AccountProjectsDto>();
            ProjectDetailsStore = new InMemoryDatabase<Guid, ProjectDetailsDto>();
            EnvironmentDetailsStore = new InMemoryDatabase<string, EnvironmentDetailsDto>();
            ToggleDetailsStore = new InMemoryDatabase<string, ToggleDetailsDto>();
            EnvironmentStatesStore = new InMemoryDatabase<string, EnvironmentStateDto>();

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

        protected IDatabase<Guid, AccountProjectsDto> AccountProjectsStore { get; }

        protected IDatabase<Guid, ProjectDetailsDto> ProjectDetailsStore { get; }

        protected IDatabase<string, EnvironmentDetailsDto> EnvironmentDetailsStore { get; set; }

        protected IDatabase<string, ToggleDetailsDto> ToggleDetailsStore { get; set; }

        protected IDatabase<string, EnvironmentStateDto> EnvironmentStatesStore { get; set; }

        protected StubbedRepository StubbedRepository { get; set; }

        protected Exception ThrownException { get; set; }

        protected abstract void RegisterHandlers(Router router);

        protected void GivenWePublish(IEvent @event)
        {
            _publisher.Publish(@event).GetAwaiter().GetResult();
        }
    }
}
