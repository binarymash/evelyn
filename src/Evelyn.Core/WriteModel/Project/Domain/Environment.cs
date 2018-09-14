namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;

    public class Environment : ScopedEntity
    {
        public Environment()
        {
            LastModifiedVersion = -1;
        }

        public Environment(string key, string name, DateTimeOffset occurredAt, int lastModifiedVersion, string userId)
            : this()
        {
            Key = key;
            Name = name;
            Created = occurredAt;
            CreatedBy = userId;
            LastModified = occurredAt;
            LastModifiedBy = CreatedBy;
            LastModifiedVersion = lastModifiedVersion;
        }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }
    }
}
