using System.Collections.Generic;

namespace Evelyn.Agent.UnitTests.Features.Locations.Get
{
    static class TestDirectories
    {
        public static NoLocations NoLocations(string parentPath) => new NoLocations(parentPath);

        public static SingleLevelDirectoryWithLocations SingleLevelDirectoryWithLocations(string parentPath) => new SingleLevelDirectoryWithLocations(parentPath);
    }

    class TestDirectory
    {
        public string ParentPath { get; private set; }

        public string Name { get; private set; }

        public IEnumerable<TestDirectory> SubDirectories { get; private set; }

        public bool HasLocationFile { get; set; }

        public string FullPath => System.IO.Path.Combine(ParentPath, Name);

        public TestDirectory(string parentPath, string name, IEnumerable<TestDirectory> subDirectories = null, bool hasLocationFile = false)
        {
            ParentPath = parentPath;
            Name = name;
            SubDirectories = subDirectories ?? new List<TestDirectory>();
            HasLocationFile = hasLocationFile;
        }

        public void Create()
        {
            System.IO.Directory.CreateDirectory(FullPath);

            foreach (var subDirectory in SubDirectories)
            {
                subDirectory.Create();
            }

            if (HasLocationFile)
            {
                System.IO.File.CreateText(System.IO.Path.Combine(FullPath, "evelyn.json"));
            }
        }
    }

    class NoLocations : TestDirectory
    {
        public NoLocations(string parentPath) : base(parentPath, "NoLocations")
        {
        }
    }

    class SingleLevelDirectoryWithLocations : TestDirectory
    {
        public SingleLevelDirectoryWithLocations(string parentPath) : base(parentPath, "SingleLevelDirectoryWithLocations", null, true)
        {
        }
    }
}
