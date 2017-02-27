namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.Support
{
    public class SomePayload
    {
        public SomePayload(string someValue)
        {
            SomeValue = someValue;
        }

        public string SomeValue { get; private set; }
    }
}
