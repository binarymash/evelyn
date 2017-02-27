namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ExceptionHandlingSpecs
{
    using BinaryMash.Responses;
    using Moq;
    using TestStack.BDDfy;
    using Xunit;

    public class ResponseExceptionHandlingSpecs : ExceptionHandlingSpecs<Support.MyRequest, Response>
    {
        [Fact]
        public void ExceptionNotThrownByNextHandler()
        {
            this.Given(_ => GivenTheNextHandlerDoesNotThrowAnException())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsCalled())
                .And(_ => ThenTheResponseFromTheNextHandlerIsReturned())
                .And(_ => ThenNoErrorIsLogged())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByNextHandler()
        {
            this.Given(_ => GivenTheNextHandlerThrowsAnException())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsCalled())
                .And(_ => ThenTheExceptionIsLogged())
                .And(_ => ThenAnInternalErrorIsReturned())
                .BDDfy();
        }

        private void GivenTheNextHandlerDoesNotThrowAnException()
        {
            ResponseFromNext = BuildResponse.WithNoPayload().Create();
            Next.Setup(n => n()).ReturnsAsync(ResponseFromNext);
        }
    }
}
