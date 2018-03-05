namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;
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

            ProjectsStore = new InMemoryDatabase<ProjectListDto>();
            ProjectDetailsStore = new InMemoryDatabase<ProjectDetailsDto>();
            EnvironmentDetailsStore = new InMemoryDatabase<EnvironmentDetailsDto>();
            ToggleDetailsStore = new InMemoryDatabase<ToggleDetailsDto>();

            ReadModelFacade = new DatabaseReadModelFacade(
                ProjectsStore,
                ProjectDetailsStore,
                EnvironmentDetailsStore,
                ToggleDetailsStore);

            var router = new Router();
            RegisterHandlers(router);
            _publisher = router;
        }

        protected Fixture DataFixture { get; }

        protected IReadModelFacade ReadModelFacade { get; }

        protected IDatabase<ProjectListDto> ProjectsStore { get; }

        protected IDatabase<ProjectDetailsDto> ProjectDetailsStore { get; }

        protected IDatabase<EnvironmentDetailsDto> EnvironmentDetailsStore { get; set; }

        protected IDatabase<ToggleDetailsDto> ToggleDetailsStore { get; set; }

        protected Exception ThrownException { get; set; }

        protected abstract void RegisterHandlers(Router router);

        protected void GivenWePublish(IEvent @event)
        {
            _publisher.Publish(@event).GetAwaiter().GetResult();
        }
    }
}
