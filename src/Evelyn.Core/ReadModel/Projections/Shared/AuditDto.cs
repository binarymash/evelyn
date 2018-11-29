namespace Evelyn.Core.ReadModel.Projections.Shared
{
    using System;
    using Newtonsoft.Json;

    public class AuditDto
    {
        [JsonConstructor]
        protected AuditDto(DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            Created = created;
            CreatedBy = createdBy;
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
            Version = version;
        }

        public DateTimeOffset Created { get; protected set; }

        public string CreatedBy { get; protected set; }

        public DateTimeOffset LastModified { get; protected set; }

        public string LastModifiedBy { get; protected set; }

        public long Version { get; protected set; }

        public static AuditDto Create(DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            return new AuditDto(occurredAt, initiatedBy, occurredAt, initiatedBy, newVersion);
        }

        public void Update(DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            LastModified = occurredAt;
            LastModifiedBy = initiatedBy;
            Version = newVersion;
        }
    }
}
