namespace Evelyn.Management.Api.Rest.Write.ToggleStates.Messages
{
    using System;

    public class ChangeToggleState
    {
        public ChangeToggleState(Guid environmentId, Guid toggleId, int expectedVersion, string state)
        {
            EnvironmentId = environmentId;
            ToggleId = toggleId;
            ExpectedVersion = expectedVersion;
            State = state;
        }

        public Guid EnvironmentId { get; }

        public Guid ToggleId { get; }

        public int ExpectedVersion { get; }

        public string State { get; }
    }
}
