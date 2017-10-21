﻿namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class FlipToggle : ICommand
    {
        public FlipToggle(Guid applicationId, Guid environmentId, Guid toggleId)
        {
            ApplicationId = applicationId;
            EnvironmentId = environmentId;
            ToggleId = toggleId;
        }

        public Guid ApplicationId { get; set; }

        public int ExpectedVersion { get; set; }

        public Guid EnvironmentId { get; set; }

        public Guid ToggleId { get; set; }
    }
}