namespace Evelyn.Core.WriteModel.Project.Commands.DeleteToggle
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid projectId, string key, int expectedToggleVersion)
            : base(userId)
        {
            Key = key;
            ProjectId = projectId;
            ExpectedToggleVersion = expectedToggleVersion;
        }

        public int ExpectedToggleVersion { get; set; }

        public Guid ProjectId { get; set; }

        public string Key { get; set; }
    }
}
