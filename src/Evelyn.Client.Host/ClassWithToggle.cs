namespace Evelyn.Client.Host
{
    using System.Threading;
    
    public class ClassWithToggle
    {
        private readonly IEvelynClient _evelyn;
        private bool? _previousToggleState = null;

        public ClassWithToggle(IEvelynClient evelyn)
        {
            _evelyn = evelyn;
        }

        public void DoSomething()
        {
            var currentToggleState = _evelyn.GetToggleState("my-first-toggle");
            if (!_previousToggleState.HasValue || currentToggleState != _previousToggleState.Value)
            {
                _previousToggleState = currentToggleState;
                System.Console.WriteLine($"Toggle state changed. This code is called when the toggle is {(currentToggleState ? "ON" : "OFF")}");
            }

            Thread.Sleep(500);
        }
    }
}
