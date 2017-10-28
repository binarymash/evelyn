﻿namespace Evelyn.Core.Tests.ReadModel.ApplicationList
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.Events;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class ApplicationListHandlerSpecs : HandlerSpecs
    {
        private List<IEvent> _eventsApplication1;
        private List<IEvent> _eventsApplication2;

        private ApplicationCreated _event1;
        private ApplicationCreated _event2;

        public ApplicationListHandlerSpecs()
        {
            _eventsApplication1 = new List<IEvent>();
            _eventsApplication2 = new List<IEvent>();
        }

        [Fact]
        public void ApplicationCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .When(_ => GivenAllEventsArePublished())
                .Then(_ => ThenTheApplicationIsAddedToTheApplicationList())
                .BDDfy();
        }

        [Fact]
        public void MultipleApplicationsCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .And(_ => GivenAnotherApplicationIsCreated())
                .When(_ => GivenAllEventsArePublished())
                .Then(_ => ThenBothApplicationsAreInTheApplicationList())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new ApplicationListHandler(ApplicationsStore);
            router.RegisterHandler<ApplicationCreated>(handler.Handle);
        }

        private void GivenAnApplicationIsCreated()
        {
            _event1 = DataFixture.Create<ApplicationCreated>();
            _event1.Version = _eventsApplication1.Count + 1;

            _eventsApplication1.Add(_event1);
            Events.Add(_event1);
        }

        private void GivenAnotherApplicationIsCreated()
        {
            _event2 = DataFixture.Create<ApplicationCreated>();
            _event2.Version = _eventsApplication2.Count + 1;

            _eventsApplication2.Add(_event2);
            Events.Add(_event2);
        }

        private void ThenTheApplicationIsAddedToTheApplicationList()
        {
            var applications = ReadModelFacade.GetApplications().ToList();
            applications.Count.ShouldBe(1);
            applications[0].Id.ShouldBe(_event1.Id);
            applications[0].Name.ShouldBe(_event1.Name);
        }

        private void ThenBothApplicationsAreInTheApplicationList()
        {
            ReadModelFacade.GetApplications().Count().ShouldBe(2);

            ThenThereIsAnApplicationInTheListFor(_event1);
            ThenThereIsAnApplicationInTheListFor(_event2);
        }

        private void ThenThereIsAnApplicationInTheListFor(ApplicationCreated ev)
        {
            ApplicationsStore.Get().ShouldContain(application =>
                application.Id == ev.Id &&
                application.Name == ev.Name);
        }
    }
}
