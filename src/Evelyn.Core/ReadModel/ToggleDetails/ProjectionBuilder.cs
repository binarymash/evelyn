namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ToggleDetailsDto> Invoke(ProjectionBuilderRequest request, CancellationToken token = default)
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId, token);
                var toggle = project.Toggles.FirstOrDefault(t => t.Key == request.ToggleKey);
                if (toggle == null)
                {
                    return null;
                }

                var dto = new ToggleDetailsDto(request.ProjectId, toggle.ScopedVersion, toggle.Key, toggle.Name, toggle.Created, toggle.CreatedBy, toggle.LastModified, toggle.LastModifiedBy);

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
