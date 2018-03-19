﻿namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;

    public class EnvironmentDetailsDto : DtoRoot
    {
        public EnvironmentDetailsDto(Guid projectId, string key, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
            : base(-1, created, createdBy, lastModified, lastModifiedBy)
        {
            Key = key;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }
    }
}
