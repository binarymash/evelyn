namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;

    public class ProjectionBuilderRequest
    {
        public ProjectionBuilderRequest(Guid projectId, string toggleKey)
        {
            ProjectId = projectId;
            ToggleKey = toggleKey;
        }

        public Guid ProjectId { get; }

        public string ToggleKey { get; }
    }
}
