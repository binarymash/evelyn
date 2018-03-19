namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;

    public class ProjectionBuilderRequest
    {
        public ProjectionBuilderRequest(Guid projectId)
        {
            ProjectId = projectId;
        }

        public Guid ProjectId { get; }
    }
}
