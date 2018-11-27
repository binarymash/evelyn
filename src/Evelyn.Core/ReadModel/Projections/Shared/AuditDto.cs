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

        public static AuditDto Create(DateTimeOffset created, string createdBy, long version)
        {
            return Create(created, createdBy, created, createdBy, version);
        }

        public static AuditDto Create(DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            return new AuditDto(created, createdBy, created, createdBy, version);
        }

        public void Update(DateTimeOffset lastModified, string lastModifiedBy, long lastModifiedVersion)
        {
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
            Version = lastModifiedVersion;
        }
    }
}
