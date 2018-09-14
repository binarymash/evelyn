namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Globalization;
    using CQRSlite.Domain.Exception;
    using Newtonsoft.Json;

    [JsonObject]
    public class Toggle : ScopedEntity
    {
        public Toggle()
        {
        }

        public Toggle(string key, string name, DateTimeOffset occurredAt, int lastModifiedVersion, string userId)
        {
            Name = name;
            Key = key;
            Created = occurredAt;
            CreatedBy = userId;
            LastModified = occurredAt;
            LastModifiedBy = userId;
            LastModifiedVersion = lastModifiedVersion;
        }

        public string Name { get; private set; }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }

        public string DefaultValue => default(bool).ToString(CultureInfo.InvariantCulture);
    }
}
