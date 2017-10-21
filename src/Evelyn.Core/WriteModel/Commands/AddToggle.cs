﻿namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class AddToggle : ICommand
    {
        public AddToggle(Guid applicationId, Guid id, string name, string key)
        {
            Id = id;
            Name = name;
            Key = key;
            ApplicationId = applicationId;
        }

        public int ExpectedVersion { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}