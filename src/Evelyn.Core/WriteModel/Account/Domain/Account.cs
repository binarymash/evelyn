﻿namespace Evelyn.Core.WriteModel.Account.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Events;
    using Newtonsoft.Json;
    using Project.Domain;

    public class Account : EvelynAggregateRoot
    {
        [JsonProperty("Projects")]
        private readonly List<Guid> _projects;

        public Account()
        {
            Version = -1;
            _projects = new List<Guid>();
        }

        public Account(string userId, Guid accountId)
            : this()
        {
            ApplyChange(new AccountRegistered(userId, accountId, DateTimeOffset.UtcNow));
        }

        [JsonIgnore]
        public IEnumerable<Guid> Projects => _projects.ToList();

        public Project CreateProject(string userId, Guid projectId, string name, int? expectedVersion)
        {
            AssertVersion(expectedVersion);

            if (_projects.Contains(projectId))
            {
                throw new InvalidOperationException($"There is already a project with the id {projectId}");
            }

            ApplyChange(new ProjectCreated(userId, Id, projectId, DateTimeOffset.UtcNow));

            return new Project(userId, this.Id, projectId, name);
        }

        public void DeleteProject(string userId, Guid projectId)
        {
            if (_projects.Contains(projectId))
            {
                ApplyChange(new ProjectDeleted(userId, Id, projectId, DateTimeOffset.UtcNow));
            }
        }

        private void Apply(AccountRegistered @event)
        {
            Id = @event.Id;
            Created = @event.OccurredAt;
            CreatedBy = @event.UserId;
            LastModified = @event.OccurredAt;
            LastModifiedBy = @event.UserId;
            LastModifiedVersion = CalculateLastModifiedVersion();
        }

        private void Apply(ProjectCreated @event)
        {
            _projects.Add(@event.ProjectId);
            LastModified = @event.OccurredAt;
            LastModifiedBy = @event.UserId;
            LastModifiedVersion = CalculateLastModifiedVersion();
        }

        private void Apply(ProjectDeleted @event)
        {
            _projects.Remove(@event.ProjectId);
            LastModified = @event.OccurredAt;
            LastModifiedBy = @event.UserId;
            LastModifiedVersion = CalculateLastModifiedVersion();
        }
    }
}
