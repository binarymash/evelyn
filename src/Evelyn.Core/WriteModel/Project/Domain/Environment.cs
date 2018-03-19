namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;

    public class Environment
    {
        public Environment()
        {
        }

        public Environment(string key, DateTimeOffset created)
            : this()
        {
            Key = key;
            Created = created;
            LastModified = created;
        }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
