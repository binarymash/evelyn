namespace Evelyn.Core.ReadModel.Projections.Shared
{
    using System;
    using Newtonsoft.Json;

    public class EventAuditDto
    {
        [JsonConstructor]
        protected EventAuditDto(DateTimeOffset lastModified, string lastModifiedBy, long eventVersion, long streamVersion)
        {
            OccurredAt = lastModified;
            InitiatedBy = lastModifiedBy;
            EventVersion = eventVersion;
            StreamVersion = streamVersion;
        }

        public DateTimeOffset OccurredAt { get; protected set; }

        public string InitiatedBy { get; protected set; }

        public long EventVersion { get; protected set; }

        public long StreamVersion { get; protected set; }

        public static EventAuditDto Create(DateTimeOffset occurredAt, string initiatedBy, long eventVersion, long streamVersion)
        {
            return new EventAuditDto(occurredAt, initiatedBy, eventVersion, streamVersion);
        }
    }
}
