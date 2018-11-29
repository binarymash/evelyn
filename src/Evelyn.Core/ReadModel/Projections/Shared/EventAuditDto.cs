namespace Evelyn.Core.ReadModel.Projections.Shared
{
    using System;
    using Newtonsoft.Json;

    public class EventAuditDto
    {
        [JsonConstructor]
        protected EventAuditDto(DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            OccurredAt = lastModified;
            InitiatedBy = lastModifiedBy;
            NewVersion = version;
        }

        public DateTimeOffset OccurredAt { get; protected set; }

        public string InitiatedBy { get; protected set; }

        public long NewVersion { get; protected set; }

        public static EventAuditDto Create(DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            return new EventAuditDto(occurredAt, initiatedBy, newVersion);
        }
    }
}
