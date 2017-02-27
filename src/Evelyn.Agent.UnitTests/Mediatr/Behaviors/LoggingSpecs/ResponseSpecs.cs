﻿namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.LoggingSpecs
{
    using BinaryMash.Responses;
    using Evelyn.Agent.UnitTests.Mediatr.Behaviors.LoggingSpecs.Support;
    using TestStack.BDDfy;
    using Xunit;

    public class ResponseSpecs : LoggingBehaviourSpecs<MyRequest, Response>
    {
        [Fact]
        public void NullRequest()
        {
            this.Given(_ => GivenANullRequest())
                .And(_ => GivenTheNextHandlerWillReturnAResponse())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheRequestIsLogged())
                .And(_ => ThenTheNextHandlerIsCalled())
                .And(_ => ThenTheResponseIsLogged())
                .And(_ => ThenTheResponseFromTheNextHandlerIsReturned())
                .BDDfy();
        }

        [Fact]
        public void LogsRequestAndResponse()
        {
            this.Given(_ => GivenARequest())
                .And(_ => GivenTheNextHandlerWillReturnAResponse())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheRequestIsLogged())
                .And(_ => ThenTheNextHandlerIsCalled())
                .And(_ => ThenTheResponseIsLogged())
                .And(_ => ThenTheResponseFromTheNextHandlerIsReturned())
                .BDDfy();
        }

        [Fact]
        public void NextHandlerThrowsException()
        {
            this.Given(_ => GivenARequest())
                .And(_ => GivenTheNextHandlerWillThrowAnException())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheRequestIsLogged())
                .And(_ => ThenTheNextHandlerIsCalled())
                .Then(_ => ThenTheResponseIsNotLogged())
                .And(_ => ThenTheExceptionIsRethrown())
                .BDDfy();
        }

        private void GivenANullRequest()
        {
            GivenARequest(null);
        }

        private void GivenARequest()
        {
            GivenARequest(new MyRequest { MyProperty = "test" });
        }

        private void GivenTheNextHandlerWillReturnAResponse()
        {
            GivenTheNextHandlerWillReturnAResponse(
                BuildResponse
                    .WithNoPayload()
                    .Create());
        }

        private void ThenTheRequestIsLogged()
        {
            ThenInformationIsLogged("Handling MyRequest");
        }

        private void ThenTheResponseIsLogged()
        {
            ThenInformationIsLogged("Handled Response");
        }

        private void ThenTheResponseIsNotLogged()
        {
            ThenInformationIsNotLogged("Handled Response");
        }
    }
}
