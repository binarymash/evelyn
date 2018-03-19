namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;

    public class ToggleState
    {
        public ToggleState()
        {
            Version = -1;
        }

        public ToggleState(string key, string value, DateTimeOffset occurredAt, string userId)
            : this()
        {
            Key = key;
            Value = value;
            Version = 0;
            Created = occurredAt;
            CreatedBy = userId;
            LastModified = occurredAt;
            LastModifiedBy = userId;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public int Version { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }

        public void SetState(string value, DateTimeOffset occurredAt, string userId)
        {
            Value = value;
            LastModified = occurredAt;
            LastModifiedBy = userId;
            Version++;
        }
    }
}
