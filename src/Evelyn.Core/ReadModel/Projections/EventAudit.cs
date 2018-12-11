namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using Newtonsoft.Json;

    public class EventAudit
    {
        [JsonConstructor]
        protected EventAudit(DateTimeOffset lastModified, string lastModifiedBy, long eventVersion, long streamPosition)
        {
            OccurredAt = lastModified;
            InitiatedBy = lastModifiedBy;
            EventVersion = eventVersion;
            StreamPosition = streamPosition;
        }

        public DateTimeOffset OccurredAt { get; protected set; }

        public string InitiatedBy { get; protected set; }

        public long EventVersion { get; protected set; }

        public long StreamPosition { get; protected set; }

        public static EventAudit Create(DateTimeOffset occurredAt, string initiatedBy, long eventVersion, long streamPosition)
        {
            return new EventAudit(occurredAt, initiatedBy, eventVersion, streamPosition);
        }
    }
}
