namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        public Projection(ProjectionAuditDto audit, Model.Toggle toggle)
            : base(audit)
        {
            Toggle = toggle;
        }

        public Model.Toggle Toggle { get; private set; }

        public static Projection Create(EventAuditDto eventAudit, Model.Toggle toggle)
        {
            return new Projection(ProjectionAuditDto.Create(eventAudit), toggle);
        }

        public static string StoreKey(Guid projectId, string toggleKey)
        {
            return $"{nameof(ToggleDetails)}-{projectId}-{toggleKey}";
        }
    }
}
