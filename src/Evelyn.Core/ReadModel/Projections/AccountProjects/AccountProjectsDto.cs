namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class AccountProjectsDto : DtoRoot
    {
        [JsonProperty("projects")]
        private readonly List<ProjectListDto> _projects;

        [JsonConstructor]
        private AccountProjectsDto(Guid accountId, IEnumerable<ProjectListDto> projects, AuditDto audit)
            : base(audit)
        {
            AccountId = accountId;
            _projects = projects?.ToList() ?? new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        [JsonIgnore]
        public IEnumerable<ProjectListDto> Projects => _projects.ToList();

        public static string StoreKey(Guid accountId)
        {
            return $"{nameof(AccountProjectsDto)}-{accountId}";
        }

        public static AccountProjectsDto Create(Guid accountId, DateTimeOffset created, string createdBy, long version)
        {
            return new AccountProjectsDto(accountId, new List<ProjectListDto>(), AuditDto.Create(created, createdBy, version));
        }

        public void AddProject(Guid projectId, string name, DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            Audit.Update(lastModified, lastModifiedBy, version);

            var project = new ProjectListDto(projectId, name);
            _projects.Add(project);
        }

        public void DeleteProject(Guid projectId, DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            Audit.Update(lastModified, lastModifiedBy, version);

            var project = _projects.Single(p => p.Id == projectId);
            _projects.Remove(project);
        }

        internal void SetProjectName(Guid projectId, string name, DateTimeOffset lastModified, string lastModifiedBy, long version)
        {
            Audit.Update(lastModified, lastModifiedBy, version);

            _projects
                .Single(p => p.Id == projectId)
                .SetName(name);
        }
    }
}
