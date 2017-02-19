namespace Evelyn.Agent.UnitTests.SetUp
{
    using System.Collections.Generic;

    internal class SingleLevelDirectoryWithLocations : TestDirectory
    {
        public SingleLevelDirectoryWithLocations(string parentPath)
            : base(parentPath, "SingleLevelDirectoryWithLocations", null, true)
        {
        }
    }
}
