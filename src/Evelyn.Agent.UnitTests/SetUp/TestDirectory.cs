namespace Evelyn.Agent.UnitTests.SetUp
{
    using System.Collections.Generic;

    internal class TestDirectory
    {
        public TestDirectory(string parentPath, string name, IEnumerable<TestDirectory> subDirectories = null, bool hasLocationFile = false)
        {
            ParentPath = parentPath;
            Name = name;
            SubDirectories = subDirectories ?? new List<TestDirectory>();
            HasLocationFile = hasLocationFile;
        }

        public string ParentPath { get; private set; }

        public string Name { get; private set; }

        public IEnumerable<TestDirectory> SubDirectories { get; private set; }

        public bool HasLocationFile { get; set; }

        public string FullPath => System.IO.Path.Combine(ParentPath, Name);

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
}
