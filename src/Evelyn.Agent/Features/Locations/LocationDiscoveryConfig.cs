namespace Evelyn.Agent.Features.Locations
{
    using System.Collections.Generic;

    public class LocationDiscoveryConfig
    {
        public LocationDiscoveryConfig()
        {
            WatchedDirectories = new List<string>();
            LocationFileSearchPattern = "evelyn.json";
        }

        public List<string> WatchedDirectories { get; set; }

        public string LocationFileSearchPattern { get; set; }
    }
}
