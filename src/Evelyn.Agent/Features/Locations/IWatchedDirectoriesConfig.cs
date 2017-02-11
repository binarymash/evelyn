namespace Evelyn.Agent.Features.Locations
{
    using System.Collections.Generic;

    public interface IWatchedDirectoriesConfig
    {
        IReadOnlyCollection<string> WatchedDirectories { get; }

        string LocationFileSearchPattern { get; }
    }
}
