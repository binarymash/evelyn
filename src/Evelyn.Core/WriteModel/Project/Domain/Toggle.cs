namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Globalization;

    public class Toggle
    {
        public Toggle()
        {
        }

        public Toggle(string key, string name, DateTimeOffset occurredAt, string userId)
        {
            Name = name;
            Key = key;
            Created = occurredAt;
            CreatedBy = userId;
            LastModified = occurredAt;
            LastModifiedBy = userId;
        }

        public string Name { get; }

        public string Key { get; }

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }

        public string DefaultValue => default(bool).ToString(CultureInfo.InvariantCulture);
    }
}
