namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.Support
{
    using MediatR;

    public class SomeRequest : IRequest
    {
        public SomeRequest(bool mustBeTrue, int mustBeGreaterThan5)
        {
            MustBeTrue = mustBeTrue;
            MustBeGreaterThan5 = mustBeGreaterThan5;
        }

        public bool MustBeTrue { get; private set; }

        public int MustBeGreaterThan5 { get; private set; }
    }
}
