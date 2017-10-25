namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using System.Collections.Generic;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Infrastructure;

    public abstract class HandlerSpecs
    {
        private readonly IEventPublisher _publisher;

        public HandlerSpecs()
        {
            Events = new List<IEvent>();

            ApplicationsStore = new InMemoryDatabase<ApplicationListDto>();
            ApplicationDetailsStore = new InMemoryDatabase<ApplicationDetailsDto>();
            EnvironmentDetailsStore = new InMemoryDatabase<EnvironmentDetailsDto>();

            ReadModelFacade = new InMemoryReadModelFacade(ApplicationsStore, ApplicationDetailsStore);

            var router = new Router();
            RegisterHandlers(router);
            _publisher = router;
        }

        protected IReadModelFacade ReadModelFacade { get; }

        protected List<IEvent> Events { get; }

        protected IDatabase<ApplicationListDto> ApplicationsStore { get; }

        protected IDatabase<ApplicationDetailsDto> ApplicationDetailsStore { get; }

        protected IDatabase<EnvironmentDetailsDto> EnvironmentDetailsStore { get; set; }

        protected abstract void RegisterHandlers(Router router);

        protected void WhenTheEventsArePublished()
        {
            foreach (var ev in Events)
            {
                _publisher.Publish(ev).GetAwaiter().GetResult();
            }
        }
    }
}
