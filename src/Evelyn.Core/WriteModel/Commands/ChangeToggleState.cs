namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class ChangeToggleState : ICommand
    {
        public ChangeToggleState(Guid applicationId, Guid environmentId, Guid toggleId, string value, int? expectedVersion = null)
        {
            ApplicationId = applicationId;
            EnvironmentId = environmentId;
            ToggleId = toggleId;
            Value = value;
            ExpectedVersion = expectedVersion;
        }

        public Guid ApplicationId { get; set; }

        public int? ExpectedVersion { get; set; }

        public Guid EnvironmentId { get; set; }

        public Guid ToggleId { get; set; }

        public string Value { get; set; }
    }
}
