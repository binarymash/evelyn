namespace Evelyn.Client.Domain
{
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentState
    {
        private readonly List<ToggleState> _toggleStates;

        public EnvironmentState(int version, IEnumerable<ToggleState> toggleStates)
        {
            _toggleStates = toggleStates.ToList();
            Version = version;
        }

        public int Version { get; private set; }

        public IEnumerable<ToggleState> ToggleStates => _toggleStates;
    }
}
