namespace Evelyn.Core.ReadModel.Projections.ClientEnvironmentState
{
    using System;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAudit audit, Model.EnvironmentState environmentState)
            : base(audit)
        {
            EnvironmentState = environmentState;
        }

        public Model.EnvironmentState EnvironmentState { get; set; }

        public static Projection Create(EventAudit eventAudit, Model.EnvironmentState environmentState)
        {
            return new Projection(ProjectionAudit.Create(eventAudit), environmentState);
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(ClientEnvironmentState)}-{projectId}-{environmentKey}";
        }
    }
}
