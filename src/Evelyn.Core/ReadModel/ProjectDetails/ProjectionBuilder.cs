namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, ProjectDetailsDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProjectDetailsDto> Invoke(ProjectionBuilderRequest request, CancellationToken token)
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId, token);
                var environments = project.Environments.Select(e => new EnvironmentListDto(e.Key));
                var toggles = project.Toggles.Select(t => new ToggleListDto(t.Key, t.Name));
                var dto = new ProjectDetailsDto(project.Id, project.Name, environments, toggles, project.Version, project.Created, project.LastModified);

                return dto;
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
