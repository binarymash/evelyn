namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    using System;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAudit audit, Model.Project project)
            : base(audit)
        {
            Project = project;
        }

        public Model.Project Project { get; private set; }

        public static Projection Create(EventAudit eventAudit, Model.Project project)
        {
            return new Projection(ProjectionAudit.Create(eventAudit), project);
        }

        public static string StoreKey(Guid projectId)
        {
            return $"{nameof(ProjectDetails)}-{projectId}";
        }
    }
}
