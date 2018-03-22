namespace Evelyn.Core.WriteModel.Project.Commands
{
    using System;

    public class ChangeToggleState : Command
    {
        public ChangeToggleState(string userId, Guid projectId, string environmentKey, string toggleKey, string value, int expectedVersion)
            : base(userId)
        {
            ProjectId = projectId;
            EnvironmentKey = environmentKey;
            ToggleKey = toggleKey;
            Value = value;
            ExpectedVersion = expectedVersion;
        }

        public Guid ProjectId { get; set; }

        public int ExpectedVersion { get; set; }

        public string EnvironmentKey { get; set; }

        public string ToggleKey { get; set; }

        public string Value { get; set; }
    }
}
