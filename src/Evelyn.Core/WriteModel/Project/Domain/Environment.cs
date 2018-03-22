namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;

    public class Environment : IScopedEntity
    {
        public Environment()
        {
            ScopedVersion = -1;
        }

        public Environment(string key, DateTimeOffset occurredAt, string userId)
            : this()
        {
            Key = key;
            Created = occurredAt;
            CreatedBy = userId;
            LastModified = occurredAt;
            LastModifiedBy = CreatedBy;
            ScopedVersion = 0;
        }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }

        public int ScopedVersion { get; private set; }
    }
}
