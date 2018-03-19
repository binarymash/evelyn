namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;

    public class ToggleState
    {
        public ToggleState()
        {
            Version = -1;
        }

        public ToggleState(string key, string value, DateTimeOffset created)
            : this()
        {
            Key = key;
            Value = value;
            Version = 0;
            Created = created;
            LastModified = created;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public int Version { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public void SetState(string value, DateTimeOffset modified)
        {
            Value = value;
            LastModified = modified;
            Version++;
        }
    }
}
