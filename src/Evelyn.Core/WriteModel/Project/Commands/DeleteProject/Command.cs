namespace Evelyn.Core.WriteModel.Project.Commands.DeleteProject
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid accountId, Guid projectId, int? expectedProjectVersion)
            : base(userId)
        {
            AccountId = accountId;
            ProjectId = projectId;
            ExpectedProjectVersion = expectedProjectVersion;
        }

        public Guid AccountId { get; set; }

        public Guid ProjectId { get; set; }

        public int? ExpectedProjectVersion { get; set; }
    }
}
