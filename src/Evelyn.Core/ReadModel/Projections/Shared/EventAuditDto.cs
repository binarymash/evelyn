namespace Evelyn.Core.ReadModel.Projections.Shared
{
    using System;
    using Newtonsoft.Json;

    public class EventAuditDto
    {
        [JsonConstructor]
        protected EventAuditDto(DateTimeOffset lastModified, string lastModifiedBy, long aggregateRootVersion, long streamVersion)
        {
            OccurredAt = lastModified;
            InitiatedBy = lastModifiedBy;
            AggregateRootVersion = aggregateRootVersion;
            StreamVersion = streamVersion;
        }

        public DateTimeOffset OccurredAt { get; protected set; }

        public string InitiatedBy { get; protected set; }

        public long AggregateRootVersion { get; protected set; }

        public long StreamVersion { get; protected set; }

        public static EventAuditDto Create(DateTimeOffset occurredAt, string initiatedBy, long aggregateRootVersion, long streamVersion)
        {
            return new EventAuditDto(occurredAt, initiatedBy, aggregateRootVersion, streamVersion);
        }
    }
}
