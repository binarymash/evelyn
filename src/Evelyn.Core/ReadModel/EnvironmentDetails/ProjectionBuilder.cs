namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, EnvironmentDetailsDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnvironmentDetailsDto> Invoke(ProjectionBuilderRequest request, CancellationToken token = default)
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId, token);
                var environment = project.Environments.FirstOrDefault(e => e.Key == request.EnvironmentKey);
                if (environment == null)
                {
                    return null;
                }

                var dto = new EnvironmentDetailsDto(project.Id, environment.ScopedVersion, environment.Key, environment.Name, environment.Created, environment.CreatedBy, environment.LastModified, environment.LastModifiedBy);

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
