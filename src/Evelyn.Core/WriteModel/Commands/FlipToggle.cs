using CQRSlite.Commands;
using System;

namespace Evelyn.Core.WriteModel.Commands
{
    public class FlipToggle : ICommand
    {
        public Guid ApplicationId { get; set; }

        public int ExpectedVersion { get; set; }

        public Guid EnvironmentId { get; set; }

        public Guid ToggleId { get; set; }

        public FlipToggle(Guid applicationId, Guid environmentId, Guid toggleId)
        {
            ApplicationId = applicationId;
            EnvironmentId = environmentId;
            ToggleId = toggleId;
        }
    }
}
