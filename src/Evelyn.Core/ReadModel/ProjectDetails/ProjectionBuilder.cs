namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, ProjectDetailsDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProjectDetailsDto> Invoke(ProjectionBuilderRequest request, CancellationToken token = default)
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId, token).ConfigureAwait(false);
                var environments = project.Environments.Select(e => new EnvironmentListDto(e.Key));
                var toggles = project.Toggles.Select(t => new ToggleListDto(t.Key, t.Name));
                var dto = new ProjectDetailsDto(project.Id, project.Name, environments, toggles, project.ScopedVersion, project.Created, project.CreatedBy, project.LastModified, project.LastModifiedBy);

                return dto;
            }
            catch (AggregateNotFoundException)
            {
                return null;
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
