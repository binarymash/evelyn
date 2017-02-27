namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors
{
    using System;
    using System.Linq;
    using BinaryMash.Responses;
    using Evelyn.Agent.Mediatr.Behaviors;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class ExceptionHandlingSpecs : BehaviourSpecs<object, Response>
    {
        private Mock<ILogger<Logging<object, Response>>> logger;

        public ExceptionHandlingSpecs()
        {
            logger = new Mock<ILogger<Logging<object, Response>>>();
            Behavior = new ExceptionHandling<object, Response>(logger.Object);
        }

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

        private void GivenTheNextHandlerThrowsAnException()
        {
            ExceptionThrownFromNextHandler = new Exception("boom!");
            Next.Setup(n => n()).ThrowsAsync(ExceptionThrownFromNextHandler);
        }

        private void ThenNoErrorIsLogged()
        {
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()), Times.Never);
        }

        private void ThenTheExceptionIsLogged()
        {
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    ExceptionThrownFromNextHandler,
                    It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        private void ThenAnInternalErrorIsReturned()
        {
            this.Response.Errors.Count.ShouldBe(1);
            var error = Response.Errors.First();
            error.Code.ShouldBe("InternalError");
            error.Message.ShouldBe("An internal error occurred");
        }
    }
}
