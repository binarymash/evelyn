namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class ChangeToggleState : Command
    {
        public ChangeToggleState(string userId, Guid projectId, string environmentKey, Guid toggleId, string value, int? expectedVersion = null)
            : base(userId)
        {
            ProjectId = projectId;
            EnvironmentKey = environmentKey;
            ToggleId = toggleId;
            Value = value;
            ExpectedVersion = expectedVersion;
        }

        public Guid ProjectId { get; set; }

        public int? ExpectedVersion { get; set; }

        public string EnvironmentKey { get; set; }

        public Guid ToggleId { get; set; }

        public string Value { get; set; }
    }
}
