namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAuditDto audit, Model.Environment environment)
            : base(audit)
        {
            Audit = audit;
            Environment = environment;
        }

        public Model.Environment Environment { get; private set; }

        public static Projection Create(EventAuditDto eventAudit, Model.Environment environment)
        {
            return new Projection(ProjectionAuditDto.Create(eventAudit), environment);
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentDetails)}-{projectId}-{environmentKey}";
        }
    }
}
