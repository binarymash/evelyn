namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using WriteModel.Account.Domain;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, AccountProjectsDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountProjectsDto> Invoke(ProjectionBuilderRequest request, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var account = await _repository.Get<Account>(request.AccountId, token).ConfigureAwait(false);

                var dto = new AccountProjectsDto(account.Id);

                foreach (var projectId in account.Projects)
                {
                    var project = await _repository.Get<Project>(projectId, token).ConfigureAwait(false);
                    dto.AddProject(new ProjectListDto(project.Id, project.Name));
                }

                return dto;
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
