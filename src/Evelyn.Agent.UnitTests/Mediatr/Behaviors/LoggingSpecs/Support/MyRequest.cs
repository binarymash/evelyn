namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.LoggingSpecs.Support
{
    using MediatR;

    public class MyRequest : IRequest
    {
        public string MyProperty { get; set; }
    }
}
