namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAuditDto audit, Model.Project project)
            : base(audit)
        {
            Project = project;
        }

        public Model.Project Project { get; private set; }

        public static Projection Create(EventAuditDto eventAudit, Model.Project project)
        {
            return new Projection(ProjectionAuditDto.Create(eventAudit), project);
        }

        public static string StoreKey(Guid projectId)
        {
            return $"{nameof(ProjectDetails)}-{projectId}";
        }
    }
}
