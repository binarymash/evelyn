namespace Evelyn.Core.ReadModel.Projections.EventHandlerState
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAuditDto audit)
            : base(audit)
        {
        }

        public static Projection Create(EventAuditDto eventAudit)
        {
            return new Projection(ProjectionAuditDto.Create(eventAudit));
        }

        public static string StoreKey(Type eventStreamType)
        {
            return $"{nameof(eventStreamType)}-{nameof(EventHandlerState)}";
        }
    }
}
