namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;

    public class ProjectionBuilderRequest
    {
        public ProjectionBuilderRequest(Guid projectId, string environmentKey)
        {
            ProjectId = projectId;
            EnvironmentKey = environmentKey;
        }

        public Guid ProjectId { get; }

        public string EnvironmentKey { get; }
    }
}
