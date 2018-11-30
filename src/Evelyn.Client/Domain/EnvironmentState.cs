namespace Evelyn.Client.Domain
{
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentState
    {
        private readonly List<ToggleState> _toggleStates;

        public EnvironmentState(long version, IEnumerable<ToggleState> toggleStates)
        {
            _toggleStates = toggleStates.ToList();
            Version = version;
        }

        public long Version { get; private set; }

        public IEnumerable<ToggleState> ToggleStates => _toggleStates;
    }
}
