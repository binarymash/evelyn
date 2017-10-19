﻿using CQRSlite.Commands;
using System;

namespace Evelyn.Core.WriteModel.Commands
{
    public class AddEnvironment : ICommand
    {
        public int ExpectedVersion { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public AddEnvironment(Guid applicationId, Guid id, string name, string key)
        {
            Id = id;
            Name = name;
            Key = key;
            ApplicationId = applicationId;
        }
    }
}
