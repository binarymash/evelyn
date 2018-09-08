namespace Evelyn.Client.Host
{
    using System.Threading;
    using Microsoft.Extensions.Logging;

    public class ClassWithToggle
    {
        private readonly IEvelynClient _evelyn;
        private readonly ILogger<ClassWithToggle> _logger;
        private bool? _previousToggleState = null;
        private int _iterationsSinceToggleChanged = 0;

        public ClassWithToggle(IEvelynClient evelyn, ILogger<ClassWithToggle> logger)
        {
            _evelyn = evelyn;
            _logger = logger;
        }

        public void DoSomething()
        {
            // lets get the current toggle state
            var toggleIsOn = _evelyn.GetToggleState("my-first-toggle");

            // if the value has changed then we'll log this
            LogIfToggleStateHasChanged(toggleIsOn);

            if (ItIsTimeToLogTheCurrentState())
            {
                // lets do something based on the current state of the toggle
                if (toggleIsOn)
                {
                    LogNewCode();
                }
                else
                {
                    LogOldCode();
                }
            }

            Thread.Sleep(500);
        }

        private void LogIfToggleStateHasChanged(bool currentToggleState)
        {
            if (ToggleStateHasChanged(currentToggleState))
            {
                _logger.LogInformation("Toggle state has changed.");
            }
        }

        private bool ToggleStateHasChanged(bool currentToggleState)
        {
            if (currentToggleState != _previousToggleState)
            {
                _previousToggleState = currentToggleState;
                _iterationsSinceToggleChanged = 0;
                return true;
            }

            return false;
        }

        private bool ItIsTimeToLogTheCurrentState()
        {
            return (++_iterationsSinceToggleChanged - 1) % 20 == 0;
        }

        private void LogOldCode()
        {
            _logger.LogInformation("This code is only called when the toggle is OFF.");
        }

        private void LogNewCode()
        {
            _logger.LogInformation("This code is only called when the toggle is ON.");
        }
    }
}
