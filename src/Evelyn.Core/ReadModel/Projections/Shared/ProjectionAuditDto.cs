namespace Evelyn.Core.ReadModel.Projections.Shared
{
    using System;
    using Newtonsoft.Json;

    public class ProjectionAuditDto
    {
        [JsonConstructor]
        protected ProjectionAuditDto(DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            // TODO: change to generated
            Created = created;
            CreatedBy = createdBy;
            Version = version;
        }

        public DateTimeOffset Created { get; protected set; }

        public string CreatedBy { get; protected set; }

        public long Version { get; protected set; }

        public static ProjectionAuditDto Create(EventAuditDto eventAudit)
        {
            return new ProjectionAuditDto(eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.StreamVersion);
        }
    }
}
