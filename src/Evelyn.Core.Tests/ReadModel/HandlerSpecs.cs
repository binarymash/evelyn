﻿namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
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
            DataFixture = new Fixture();

            ApplicationsStore = new InMemoryDatabase<ApplicationListDto>();
            ApplicationDetailsStore = new InMemoryDatabase<ApplicationDetailsDto>();
            EnvironmentDetailsStore = new InMemoryDatabase<EnvironmentDetailsDto>();

            ReadModelFacade = new InMemoryReadModelFacade(
                ApplicationsStore,
                ApplicationDetailsStore,
                EnvironmentDetailsStore);

            var router = new Router();
            RegisterHandlers(router);
            _publisher = router;
        }

        protected Fixture DataFixture { get; }

        protected IReadModelFacade ReadModelFacade { get; }

        protected IDatabase<ApplicationListDto> ApplicationsStore { get; }

        protected IDatabase<ApplicationDetailsDto> ApplicationDetailsStore { get; }

        protected IDatabase<EnvironmentDetailsDto> EnvironmentDetailsStore { get; set; }

        protected Exception ThrownException { get; set; }

        protected abstract void RegisterHandlers(Router router);

        protected void GivenWePublish(IEvent @event)
        {
            _publisher.Publish(@event).GetAwaiter().GetResult();
        }
    }
}