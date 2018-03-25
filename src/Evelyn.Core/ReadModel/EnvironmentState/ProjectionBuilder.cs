namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, EnvironmentStateDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnvironmentStateDto> Invoke(ProjectionBuilderRequest request, CancellationToken token = default)
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId, token);
                var environmentState = project.EnvironmentStates.First(es => es.EnvironmentKey == request.EnvironmentKey);
                var toggleStates = environmentState.ToggleStates.Select(ts => new ToggleStateDto(ts.Key, ts.Value));
                var environmentStateDto = new EnvironmentStateDto(environmentState.ScopedVersion, environmentState.Created, environmentState.CreatedBy, environmentState.LastModified, environmentState.LastModifiedBy, toggleStates);

                return environmentStateDto;
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
