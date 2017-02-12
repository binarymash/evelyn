namespace Evelyn.Agent.Features.Locations.Get
{
    using System.Collections.Generic;
    using System.Linq;
    using BinaryMash.Responses;
    using MediatR;
    using Model;

    public class Handler : IRequestHandler<Query, Response<Locations>>
    {
        private IWatchedDirectoriesConfig config;

        public Handler(IWatchedDirectoriesConfig watchedDirectoriesConfig)
        {
            config = watchedDirectoriesConfig;
        }

        public Response<Locations> Handle(Query message)
        {
            if (message == null)
            {
                return BuildResponse
                    .WithPayload(Locations.None)
                    .AndWithErrors(new Error("InvalidArgument", ""))
                    .Create();
            }

            var locations = new List<Location>();

            foreach(var directory in config.WatchedDirectories)
            {
                locations.AddRange(GetLocationsFrom(directory));
            }

            return BuildResponse
                .WithPayload(new Locations(locations))
                .Create();
        }

        private IReadOnlyCollection<Location> GetLocationsFrom(string directory)
        {
            var files = new System.IO.DirectoryInfo(directory)
                .GetFiles(config.LocationFileSearchPattern);

            return files.Select(file => new Location(file.FullName)).ToList();
        }
    }
}
