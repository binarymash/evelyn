namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentState
    {
        private readonly IList<ToggleState> _toggleStates;

        public EnvironmentState()
        {
            _toggleStates = new List<ToggleState>();
            Version = -1;
        }

        public EnvironmentState(string environmentKey, IEnumerable<ToggleState> toggleStates, DateTimeOffset created)
            : this()
        {
            EnvironmentKey = environmentKey;
            _toggleStates = toggleStates.ToList();
            Version = 0;
            Created = created;
            LastModified = created;
        }

        public string EnvironmentKey { get; private set; }

        public IEnumerable<ToggleState> ToggleStates => _toggleStates;

        public int Version { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public void AddToggleState(ToggleState toggleState)
        {
            _toggleStates.Add(toggleState);
            Version++;
        }

        public void SetToggleState(string key, string value, DateTimeOffset modified)
        {
            var toggleState = _toggleStates.First(ts => ts.Key == key);
            toggleState.SetState(value, modified);
            LastModified = modified;
            Version++;
        }
    }
}
