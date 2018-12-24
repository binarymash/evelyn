namespace Evelyn.Core.ReadModel.Projections.EventHandlerState
{
    using System;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAudit audit)
            : base(audit)
        {
        }

        public static Projection Create(EventAudit eventAudit)
        {
            return new Projection(ProjectionAudit.Create(eventAudit));
        }

        public static string StoreKey(Type eventStreamType)
        {
            return $"{nameof(eventStreamType)}-{nameof(EventHandlerState)}";
        }
    }
}
