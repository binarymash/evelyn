namespace Evelyn.Agent.UnitTests.Features.Locations.Get
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Agent.Features.Locations;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class HandlerSpecs
    {
        Evelyn.Agent.Features.Locations.Get.Handler handler;

        Evelyn.Agent.Features.Locations.IWatchedDirectoriesConfig config;

        Evelyn.Agent.Features.Locations.Get.Query query;

        List<TestDirectory> testDirectoryStructure;

        Evelyn.Agent.Features.Locations.Get.Model.Response response;

        public HandlerSpecs()
        {
            testDirectoryStructure = new List<TestDirectory>();
            query = new Agent.Features.Locations.Get.Query();
        }

        [Fact]
        public void NoWatchedDirectories()
        {
            this.Given(_ => GivenNoDirectoriesAreBeingWatched())
                .When(_ => WhenWeGetLocations())
                .Then(_ => ThenNoLocationsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void WatchedDirectoryWithNoLocations()
        {
            this.Given(_ => GivenAWatchedDirectoryHasNoLocations())
                .When(_ => WhenWeGetLocations())
                .Then(_ => ThenNoLocationsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void WatchedDirectoryWithLocations()
        {
            this.Given(_ => GivenAWatchedDirectoryHasLocations())
                .When(_ => WhenWeGetLocations())
                .Then(_ => ThenAllLocationsAreReturned())
                .BDDfy();
        }

        private void GivenNoDirectoriesAreBeingWatched()
        {
            config = new StubbedConfig();
        }

        private void GivenAWatchedDirectoryHasNoLocations()
        {
            testDirectoryStructure = new List<TestDirectory> { TestDirectories.NoLocations(@"c:\dev\test") };

            foreach (var testDirectory in testDirectoryStructure)
            {
                testDirectory.Create();
            }

            config = new StubbedConfig(testDirectoryStructure.Select(testDirectoryStructure => testDirectoryStructure.FullPath));
        }

        private void GivenAWatchedDirectoryHasLocations()
        {
            testDirectoryStructure = new List<TestDirectory> { TestDirectories.SingleLevelDirectoryWithLocations(@"c:\dev\test") };

            foreach (var testDirectory in testDirectoryStructure)
            {
                testDirectory.Create();
            }

            config = new StubbedConfig(testDirectoryStructure.Select(testDirectoryStructure => testDirectoryStructure.FullPath));
        }

        private void WhenWeGetLocations()
        {
            handler = new Agent.Features.Locations.Get.Handler(config);
            response = handler.Handle(query);
        }

        private void ThenNoLocationsAreReturned()
        {
            response.Count.ShouldBe(0);
        }

        private void ThenAllLocationsAreReturned()
        {
            var expectedDirectoriesWithLocationFiles = DirectoriesWithLocations(testDirectoryStructure);
            response.Count.ShouldBe(expectedDirectoriesWithLocationFiles.Count);
            foreach (var expectedDirectory in expectedDirectoriesWithLocationFiles)
            {
                response.Exists(r => r.Path == expectedDirectory.FullPath).ShouldBeFalse();
            }
        }

        private IReadOnlyCollection<TestDirectory> DirectoriesWithLocations(IEnumerable<TestDirectory> directories)
        {
            var directoriesWithLocationFiles = new List<TestDirectory>();

            foreach(var directory in directories)
            {
                directoriesWithLocationFiles.AddRange(DirectoriesWithLocations(directory.SubDirectories));
                if (directory.HasLocationFile)
                {
                    directoriesWithLocationFiles.Add(directory);
                }
            }

            return directoriesWithLocationFiles;
        }
    }

    class StubbedConfig : IWatchedDirectoriesConfig
    {
        List<string> watchedDirectories = new List<string>();

        public StubbedConfig() : this(new List<string>())
        {
        }

        public StubbedConfig(IEnumerable<string> watchedDirectories)
        {
            this.watchedDirectories = new List<string>(watchedDirectories);
        }

        public IReadOnlyCollection<string> WatchedDirectories => watchedDirectories;

        public string LocationFileSearchPattern => "evelyn.json";
    }
}
