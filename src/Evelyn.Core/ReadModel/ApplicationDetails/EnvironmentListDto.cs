﻿namespace Evelyn.Core.ReadModel.ApplicationDetails
{
    using System;

    public class EnvironmentListDto
    {
        public EnvironmentListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
