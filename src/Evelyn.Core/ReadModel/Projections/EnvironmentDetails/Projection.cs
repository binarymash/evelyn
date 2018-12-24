namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAudit audit, Model.Environment environment)
            : base(audit)
        {
            Audit = audit;
            Environment = environment;
        }

        public Model.Environment Environment { get; private set; }

        public static Projection Create(EventAudit eventAudit, Model.Environment environment)
        {
            return new Projection(ProjectionAudit.Create(eventAudit), environment);
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentDetails)}-{projectId}-{environmentKey}";
        }
    }
}
