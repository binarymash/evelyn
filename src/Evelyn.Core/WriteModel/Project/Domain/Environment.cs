namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;

    public class Environment
    {
        public Environment()
        {
        }

        public Environment(string key, DateTimeOffset occurredAt, string userId)
            : this()
        {
            Key = key;
            Created = occurredAt;
            CreatedBy = userId;
            LastModified = occurredAt;
            LastModifiedBy = CreatedBy;
        }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }
    }
}
