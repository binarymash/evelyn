namespace Evelyn.Management.Api.Rest.Write.ToggleStates.Messages
{
    using System;

    public class FlipToggle
    {
        public FlipToggle(Guid environmentId, Guid toggleId, int expectedVersion)
        {
            EnvironmentId = environmentId;
            ToggleId = toggleId;
            ExpectedVersion = expectedVersion;
        }

        public Guid EnvironmentId { get; }

        public Guid ToggleId { get; }

        public int ExpectedVersion { get; }
    }
}
