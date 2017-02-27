namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ExceptionHandlingSpecs.Support
{
    using MediatR;

    public class MyRequest : IRequest
    {
        public string MyProperty { get; set; }
    }
}
