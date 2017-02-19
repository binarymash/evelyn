namespace Evelyn.Agent.UnitTests.Features.Locations.Get
{
    using System.Collections.Generic;
    using System.Linq;
    using BinaryMash.Responses;
    using Evelyn.Agent.Features.Locations;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;
    using FluentValidation;
    using Moq;
    using FluentValidation.Results;
    using Evelyn.Agent.Features.Locations.Get.Model;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using Evelyn.Agent.Features.Locations.Get;

    public class HandlerSpecs
    {
        Agent.Features.Locations.Get.Handler handler;

        LocationDiscoveryConfig config;

        Query query;

        Mock<IValidator<Query>> validator;

        Mock<ILogger<Handler>> logger;

        List<TestDirectory> testDirectoryStructure;

        Response<Locations> response;

        public HandlerSpecs()
        {
            testDirectoryStructure = new List<TestDirectory>();
            query = new Query();
            validator = new Mock<IValidator<Query>>();
            logger = new Mock<ILogger<Handler>>();
        }

        [Fact]
        public void InvalidRequest()
        {
            this.Given(_ => GivenTheQueryIsInvalid())
                .When(_ => WhenWeGetLocations())
                .Then(_ => ThenAnErrorIsReturned())
                .And(_ => ThenNoLocationsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void NoWatchedDirectories()
        {
            this.Given(_ => GivenTheQueryIsValid())
                .And(_ => GivenNoDirectoriesAreBeingWatched())
                .When(_ => WhenWeGetLocations())
                .Then(_ => ThenNoLocationsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void WatchedDirectoryWithNoLocations()
        {
            this.Given(_ => GivenTheQueryIsValid())
                .And(_ => GivenAWatchedDirectoryHasNoLocations())
                .When(_ => WhenWeGetLocations())
                .Then(_ => ThenNoLocationsAreReturned())
                .And(_ => ThenNoErrorsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void WatchedDirectoryWithLocations()
        {
            this.Given(_ => GivenTheQueryIsValid())
                .And(_ => GivenAWatchedDirectoryHasLocations())
                .When(_ => WhenWeGetLocations())
                .Then(_ => ThenAllLocationsAreReturned())
                .BDDfy();
        }

        private void GivenTheQueryIsInvalid()
        {
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<Query>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("", "") }));
        }

        private void GivenTheQueryIsValid()
        {
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<Query>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        private void GivenNoDirectoriesAreBeingWatched()
        {
            config = new LocationDiscoveryConfig();
        }

        private void GivenAWatchedDirectoryHasNoLocations()
        {
            testDirectoryStructure = new List<TestDirectory> { TestDirectories.NoLocations(@"c:\dev\test") };

            foreach (var testDirectory in testDirectoryStructure)
            {
                testDirectory.Create();
            }

            config = new LocationDiscoveryConfig
            {
                WatchedDirectories = testDirectoryStructure
                    .Select(testDirectoryStructure => testDirectoryStructure.FullPath)
                    .ToList()
            };
        }

        private void GivenAWatchedDirectoryHasLocations()
        {
            testDirectoryStructure = new List<TestDirectory> { TestDirectories.SingleLevelDirectoryWithLocations(@"c:\dev\test") };

            foreach (var testDirectory in testDirectoryStructure)
            {
                testDirectory.Create();
            }

            config = new LocationDiscoveryConfig
            {
                WatchedDirectories = testDirectoryStructure
                    .Select(testDirectoryStructure => testDirectoryStructure.FullPath)
                    .ToList()
            };
        }

        private void WhenWeGetLocations()
        {
            handler = new Handler(Options.Create(config), validator.Object, logger.Object);
            response = handler.Handle(query).GetAwaiter().GetResult();
        }

        private void ThenNoErrorsAreReturned()
        {
            response.Errors.Count().ShouldBe(0);
        }

        private void ThenAnErrorIsReturned()
        {
            response.Errors.Count().ShouldBe(1);
        }

        private void ThenNoLocationsAreReturned()
        {
            response.Payload.Count.ShouldBe(0);
        }

        private void ThenAllLocationsAreReturned()
        {
            var expectedDirectoriesWithLocationFiles = DirectoriesWithLocations(testDirectoryStructure);
            response.Payload.Count.ShouldBe(expectedDirectoriesWithLocationFiles.Count);
            foreach (var expectedDirectory in expectedDirectoriesWithLocationFiles)
            {
                response.Payload.Any(location => location.Path == expectedDirectory.FullPath).ShouldBeFalse();
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
}
