namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        public Projection(ProjectionAudit audit, Model.Toggle toggle)
            : base(audit)
        {
            Toggle = toggle;
        }

        public Model.Toggle Toggle { get; private set; }

        public static Projection Create(EventAudit eventAudit, Model.Toggle toggle)
        {
            return new Projection(ProjectionAudit.Create(eventAudit), toggle);
        }

        public static string StoreKey(Guid projectId, string toggleKey)
        {
            return $"{nameof(ToggleDetails)}-{projectId}-{toggleKey}";
        }
    }
}
