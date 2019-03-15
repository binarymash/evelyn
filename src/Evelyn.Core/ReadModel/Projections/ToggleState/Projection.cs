namespace Evelyn.Core.ReadModel.Projections.ToggleState
{
    using System;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAudit audit, Model.ToggleState toggleState)
            : base(audit)
        {
            ToggleState = toggleState;
        }

        public Model.ToggleState ToggleState { get; set; }

        public static Projection Create(EventAudit eventAudit, Model.ToggleState toggleState)
        {
            return new Projection(ProjectionAudit.Create(eventAudit), toggleState);
        }

        public static string StoreKey(Guid projectId, string toggleKey)
        {
            return $"{nameof(ToggleState)}-{projectId}-{toggleKey}";
        }
    }
}
