namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Globalization;

    public class Toggle
    {
        public Toggle()
        {
        }

        public Toggle(string key, string name, DateTimeOffset created)
        {
            Name = name;
            Key = key;
            Created = created;
            LastModified = created;
        }

        public string Name { get; }

        public string Key { get; }

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string DefaultValue => default(bool).ToString(CultureInfo.InvariantCulture);
    }
}
