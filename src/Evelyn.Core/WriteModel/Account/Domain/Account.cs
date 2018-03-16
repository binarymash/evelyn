namespace Evelyn.Core.WriteModel.Account.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CQRSlite.Domain;
    using Events;
    using Project.Domain;

    public class Account : AggregateRoot
    {
        private readonly List<Guid> _projects;

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

        public IEnumerable<Guid> Projects => _projects.ToList();

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
