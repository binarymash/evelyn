namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AccountProjectsDto : DtoRoot
    {
        private readonly List<ProjectListDto> _projects;

        public AccountProjectsDto(Guid accountId, int version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, IEnumerable<ProjectListDto> projects)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
        {
            AccountId = accountId;
            _projects = projects?.ToList() ?? new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        public IEnumerable<ProjectListDto> Projects => _projects.ToList();

        public void AddProject(Guid projectId, string name, int version, DateTimeOffset lastModified, string lastModifiedBy)
        {
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
            Version = version;

            var project = new ProjectListDto(projectId, name);
            _projects.Add(project);
        }

        public void DeleteProject(Guid projectId, int version, DateTimeOffset lastModified, string lastModifiedBy)
        {
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
            Version = version;

            var project = _projects.Single(p => p.Id == projectId);
            _projects.Remove(project);
        }

        internal object Single(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
