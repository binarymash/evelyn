namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using Newtonsoft.Json;

    public class AggregateAudit
    {
        [JsonConstructor]
        protected AggregateAudit(DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, long version)
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

        public static AggregateAudit Create(EventAudit eventAudit)
        {
            return new AggregateAudit(eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.OccurredAt, eventAudit.InitiatedBy, eventAudit.EventVersion);
        }

        public void Update(EventAudit eventAudit)
        {
            LastModified = eventAudit.OccurredAt;
            LastModifiedBy = eventAudit.InitiatedBy;
            Version = eventAudit.EventVersion;
        }
    }
}
