namespace Evelyn.Agent.Features.Locations.Get
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BinaryMash.Responses;
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Model;

    public class Handler : IAsyncRequestHandler<Query, Response<Locations>>
    {
        private LocationDiscoveryConfig config;

        private IValidator<Query> validator;

        private ILogger<Handler> logger;

        public Handler(IOptions<LocationDiscoveryConfig> watchedDirectoriesConfig, IValidator<Query> validator, ILogger<Handler> logger)
        {
            config = watchedDirectoriesConfig.Value;
            this.validator = validator;
            this.logger = logger;
        }

        public async Task<Response<Locations>> Handle(Query message)
        {
            var locations = new List<Location>();

            foreach (var directory in config?.WatchedDirectories ?? new List<string>())
            {
                logger.LogInformation($"Looking for locations in {directory}");
                locations.AddRange(GetLocationsFrom(directory));
            }

            return await Task.FromResult(BuildResponse
                .WithPayload(new Locations(locations))
                .Create());
        }

        private IReadOnlyCollection<Location> GetLocationsFrom(string directory)
        {
            var files = new System.IO.DirectoryInfo(directory)
                .GetFiles(config.LocationFileSearchPattern);

            return files.Select(file => new Location(file.FullName)).ToList();
        }
    }
}
