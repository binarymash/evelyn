namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentStateAdded : Event
    {
        private readonly List<KeyValuePair<string, string>> _toggleStates;

        public EnvironmentStateAdded(string userId, Guid projectId, string environmentKey, IEnumerable<KeyValuePair<string, string>> toggleStates = null)
            : base(userId, projectId)
        {
            EnvironmentKey = environmentKey;
            _toggleStates = toggleStates?.ToList() ?? new List<KeyValuePair<string, string>>();
        }

        public string EnvironmentKey { get; private set; }

        public IEnumerable<KeyValuePair<string, string>> ToggleStates => _toggleStates;
    }
}
