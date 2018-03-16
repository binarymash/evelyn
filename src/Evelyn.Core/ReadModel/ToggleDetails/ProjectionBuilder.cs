﻿namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using WriteModel.Project.Domain;

    public class ProjectionBuilder : IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto>
    {
        private readonly IRepository _repository;

        public ProjectionBuilder(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ToggleDetailsDto> Invoke(ProjectionBuilderRequest request, CancellationToken token)
        {
            try
            {
                var project = await _repository.Get<Project>(request.ProjectId);
                var toggle = project.Toggles.First(t => t.Key == request.ToggleKey);
                var dto = new ToggleDetailsDto(request.ProjectId, toggle.Key, toggle.Name, toggle.Created);

                return dto;
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}