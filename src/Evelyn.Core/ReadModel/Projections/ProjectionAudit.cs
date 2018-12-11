namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using Newtonsoft.Json;

    public class ProjectionAudit
    {
        [JsonConstructor]
        protected ProjectionAudit(DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            // TODO: change to generated
            Created = created;
            CreatedBy = createdBy;
            Version = version;
        }

        public DateTimeOffset Created { get; protected set; }

        public string CreatedBy { get; protected set; }

        public long Version { get; protected set; }

        public static ProjectionAudit Create(EventAudit eventAudit)
        {
            return new ProjectionAudit(eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.StreamVersion);
        }
    }
}
