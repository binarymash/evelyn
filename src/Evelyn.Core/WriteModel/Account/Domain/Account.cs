namespace Evelyn.Core.WriteModel.Account.Domain
{
    using System;
    using System.Collections.Generic;
    using CQRSlite.Domain;
    using Events;
    using Project.Domain;

    public class Account : AggregateRoot
    {
        private readonly IList<Guid> _projects;

        public Account()
        {
            Version = -1;
            _projects = new List<Guid>();
        }

        public Account(string userId, Guid accountId)
            : this()
        {
            ApplyChange(new AccountRegistered(userId, accountId));
        }

        public Project CreateProject(string userId, Guid projectId, string name)
        {
            if (_projects.Contains(projectId))
            {
                throw new InvalidOperationException($"There is already a project with the id {projectId}");
            }

            ApplyChange(new ProjectCreated(userId, Id, projectId));

            return new Project(userId, this.Id, projectId, name);
        }

        private void Apply(AccountRegistered @event)
        {
        }

        private void Apply(ProjectCreated @event)
        {
            _projects.Add(@event.ProjectId);
        }
    }
}
