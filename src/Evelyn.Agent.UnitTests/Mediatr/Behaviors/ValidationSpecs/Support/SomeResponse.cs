namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.Support
{
    public class SomeResponse
    {
        public SomeResponse(string someValue)
        {
            SomeValue = someValue;
        }

        public string SomeValue { get; private set; }
    }
}
