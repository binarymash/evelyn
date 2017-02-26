namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.Support
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SomeRequest
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
