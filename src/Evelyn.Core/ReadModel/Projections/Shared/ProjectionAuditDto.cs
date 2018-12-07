namespace Evelyn.Core.ReadModel.Projections.Shared
{
    using System;
    using Newtonsoft.Json;

    public class ProjectionAuditDto
    {
        [JsonConstructor]
        protected ProjectionAuditDto(DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, long version)
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

        public static ProjectionAuditDto Create(EventAuditDto eventAudit)
        {
            return new ProjectionAuditDto(eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.StreamVersion);
        }

        public void Update(EventAuditDto eventAudit)
        {
            LastModified = eventAudit.OccurredAt;
            LastModifiedBy = eventAudit.InitiatedBy;
            Version = eventAudit.StreamVersion;
        }
    }
}
