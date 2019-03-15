namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentStateDeleted : Event
    {
        private readonly List<string> _toggleKeys;

        public EnvironmentStateDeleted(string userId, Guid projectId, string environmentKey, DateTimeOffset occurredAt, IEnumerable<string> toggleKeys = null)
            : base(userId, projectId, occurredAt)
        {
            EnvironmentKey = environmentKey;
            _toggleKeys = toggleKeys?.ToList() ?? new List<string>();
        }

        public string EnvironmentKey { get; set; }

        public IEnumerable<string> ToggleKeys => _toggleKeys;
    }
}
