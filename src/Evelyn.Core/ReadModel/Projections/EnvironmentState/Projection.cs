namespace Evelyn.Core.ReadModel.Projections.EnvironmentState
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAuditDto audit, Model.EnvironmentState environmentState)
            : base(audit)
        {
            EnvironmentState = environmentState;
        }

        public Model.EnvironmentState EnvironmentState { get; set; }

        public static Projection Create(EventAuditDto eventAudit, Model.EnvironmentState environmentState)
        {
            return new Projection(ProjectionAuditDto.Create(eventAudit), environmentState);
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentState)}-{projectId}-{environmentKey}";
        }
    }
}
