﻿namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class AddToggle : ICommand
    {
        public AddToggle(Guid applicationId, Guid id, string name, string key, int? expectedVersion = null)
        {
            Id = id;
            Name = name;
            Key = key;
            ApplicationId = applicationId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
