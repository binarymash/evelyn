namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class AccountProjectsDto : DtoRoot
    {
        [JsonProperty("Projects")]
        private readonly List<ProjectListDto> _projects;

        [JsonConstructor]
        private AccountProjectsDto(Guid accountId, int version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, IEnumerable<ProjectListDto> projects)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
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

        public static AccountProjectsDto Create(Guid accountId, DateTimeOffset created, string createdBy)
        {
            return new AccountProjectsDto(accountId, 0, created, createdBy, created, createdBy, new List<ProjectListDto>());
        }

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
    }
}
