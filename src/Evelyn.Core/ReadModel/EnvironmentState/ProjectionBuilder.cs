﻿namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, EnvironmentStateDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnvironmentStateDto> Invoke(ProjectionBuilderRequest request, CancellationToken token = new CancellationToken())
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId, token);
                var environmentState = project.EnvironmentStates.First(es => es.EnvironmentKey == request.EnvironmentKey);
                var toggleStates = environmentState.ToggleStates.Select(ts => new ToggleStateDto(ts.Version, ts.Key, ts.Value));
                var environmentStateDto = new EnvironmentStateDto(environmentState.Version, toggleStates);

                return environmentStateDto;
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}