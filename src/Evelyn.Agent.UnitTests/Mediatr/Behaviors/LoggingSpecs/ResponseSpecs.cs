namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.LoggingSpecs
{
    using System.Threading.Tasks;
    using BinaryMash.Responses;
    using TestStack.BDDfy;
    using Xunit;
    using static Evelyn.Agent.UnitTests.Mediatr.Behaviors.LoggingSpecs.ResponseSpecs;

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
                .And(_ => ThenTheResponseIsReturned())
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
                .And(_ => ThenTheResponseIsReturned())
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

        public class MyRequest
        {
            public string MyProperty { get; set; }
        }
    }
}
