namespace Evelyn.Core.WriteModel.Project.Commands.DeleteProject
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid projectId, int expectedProjectVersion)
            : base(userId)
        {
            ProjectId = projectId;
            ExpectedProjectVersion = expectedProjectVersion;
        }

        public Guid ProjectId { get; set; }

        public int ExpectedProjectVersion { get; set; }
    }
}
