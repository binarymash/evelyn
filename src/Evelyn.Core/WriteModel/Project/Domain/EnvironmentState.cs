﻿namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentState : ScopedEntity
    {
        private readonly IList<ToggleState> _toggleStates;

        public EnvironmentState()
        {
            _toggleStates = new List<ToggleState>();
            LastModifiedVersion = -1;
        }

        public EnvironmentState(string environmentKey, IEnumerable<ToggleState> toggleStates, DateTimeOffset occurredAt, int lastModifiedVersion, string userId)
            : this()
        {
            EnvironmentKey = environmentKey;
            _toggleStates = toggleStates.ToList();
            LastModifiedVersion = 0;
            Created = occurredAt;
            CreatedBy = userId;
            LastModified = occurredAt;
            LastModifiedBy = userId;
            LastModifiedVersion = lastModifiedVersion;
        }

        public string EnvironmentKey { get; private set; }

        public IEnumerable<ToggleState> ToggleStates => _toggleStates;

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }

        public void AddToggleState(ToggleState toggleState, DateTimeOffset occurredAt, int rootVersion, string userId)
        {
            _toggleStates.Add(toggleState);
            LastModified = occurredAt;
            LastModifiedBy = userId;
            LastModifiedVersion = rootVersion;
        }

        public void SetToggleState(string key, string value, DateTimeOffset occurredAt, int rootVersion, string userId)
        {
            var toggleState = _toggleStates.First(ts => ts.Key == key);
            toggleState.SetState(value, occurredAt, rootVersion, userId);
            LastModified = occurredAt;
            LastModifiedBy = userId;
            LastModifiedVersion = rootVersion;
        }

        public void DeleteToggleState(string key, DateTimeOffset occurredAt, int rootVersion, string userId)
        {
            var toggleState = _toggleStates.First(ts => ts.Key == key);
            _toggleStates.Remove(toggleState);
            LastModified = occurredAt;
            LastModifiedBy = userId;
            LastModifiedVersion = rootVersion;
        }
    }
}
