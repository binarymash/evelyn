namespace Evelyn.Agent.UnitTests.SetUp
{
    internal static class TestDirectories
    {
        public static NoLocations NoLocations(string parentPath) => new NoLocations(parentPath);

        public static SingleLevelDirectoryWithLocations SingleLevelDirectoryWithLocations(string parentPath) => new SingleLevelDirectoryWithLocations(parentPath);
    }
}
