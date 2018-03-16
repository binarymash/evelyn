namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, EnvironmentDetailsDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnvironmentDetailsDto> Invoke(ProjectionBuilderRequest request, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId, token);
                var environment = project.Environments.First(e => e.Key == request.EnvironmentKey);
                var dto = new EnvironmentDetailsDto(project.Id, environment.Key, environment.Created, environment.LastModified);

                return dto;
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
