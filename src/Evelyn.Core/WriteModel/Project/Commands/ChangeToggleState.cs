namespace Evelyn.Core.WriteModel.Project.Commands
{
    using System;

    public class ChangeToggleState : Command
    {
        public ChangeToggleState(string userId, Guid projectId, string environmentKey, string toggleKey, string value, int expectedToggleStateVersion)
            : base(userId)
        {
            ProjectId = projectId;
            EnvironmentKey = environmentKey;
            ToggleKey = toggleKey;
            Value = value;
            ExpectedToggleStateVersion = expectedToggleStateVersion;
        }

        public Guid ProjectId { get; set; }

        public int ExpectedToggleStateVersion { get; set; }

        public string EnvironmentKey { get; set; }

        public string ToggleKey { get; set; }

        public string Value { get; set; }
    }
}
