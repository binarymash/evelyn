﻿namespace Evelyn.Core.WriteModel.Commands
{
    using System;

    public class AddToggle : Command
    {
        public AddToggle(string userId, Guid projectId, string key, string name, int? expectedVersion = null)
            : base(userId)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid ProjectId { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
