namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using Newtonsoft.Json;

    public class ProjectionAudit
    {
        [JsonConstructor]
        protected ProjectionAudit(DateTimeOffset generated, long streamPosition)
        {
            Generated = generated;
            StreamPosition = streamPosition;
        }

        public DateTimeOffset Generated { get; protected set; }

        public long StreamPosition { get; protected set; }

        public static ProjectionAudit Create(EventAudit eventAudit)
        {
            return new ProjectionAudit(DateTimeOffset.UtcNow, eventAudit.StreamPosition);
        }
    }
}
