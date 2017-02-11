namespace Evelyn.Agent.Features.Locations.Get.Model
{
    public class Location
    {
        public Location(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
