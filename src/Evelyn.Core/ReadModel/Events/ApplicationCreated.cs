﻿namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ApplicationCreated : ApplicationEvent
    {
        public ApplicationCreated(Guid id, string name)
            : base(id)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}