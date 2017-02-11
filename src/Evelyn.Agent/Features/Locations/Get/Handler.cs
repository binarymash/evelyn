namespace Evelyn.Agent.Features.Locations.Get
{
    using System.Collections.Generic;
    using MediatR;
    using Model;
    using System.Linq;

    public class Handler : IRequestHandler<Query, Response>
    {
        private IWatchedDirectoriesConfig config;

        public Handler(IWatchedDirectoriesConfig watchedDirectoriesConfig)
        {
            config = watchedDirectoriesConfig;
        }

        public Response Handle(Query message)
        {
            var response = new Response();

            foreach(var directory in config.WatchedDirectories)
            {
                response.AddRange(GetLocationsFrom(directory));
            }

            return response;
        }

        private IReadOnlyCollection<Location> GetLocationsFrom(string directory)
        {
            var files = new System.IO.DirectoryInfo(directory)
                .GetFiles(config.LocationFileSearchPattern);

            return files.Select(file => new Location(file.FullName)).ToList();
        }
    }
}
