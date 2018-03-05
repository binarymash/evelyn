﻿namespace Evelyn.Core.WriteModel.Commands
{
    using System;

    public class AddToggle : Command
    {
        public AddToggle(string userId, Guid projectId, Guid id, string name, string key, int? expectedVersion = null)
            : base(userId)
        {
            Id = id;
            Name = name;
            Key = key;
            ProjectId = projectId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid ProjectId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
