namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using Newtonsoft.Json;

    public class EventAudit
    {
        [JsonConstructor]
        protected EventAudit(DateTimeOffset lastModified, string lastModifiedBy, long eventVersion, long streamVersion)
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

        public static EventAudit Create(DateTimeOffset occurredAt, string initiatedBy, long eventVersion, long streamVersion)
        {
            return new EventAudit(occurredAt, initiatedBy, eventVersion, streamVersion);
        }
    }
}
