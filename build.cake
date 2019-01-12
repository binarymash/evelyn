#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=OpenCover&version=4.6.832"
#tool "nuget:?package=ReportGenerator"
#tool "nuget:?package=GitReleaseNotes"
#addin "nuget:?package=Cake.DoInDirectory"
#addin "nuget:?package=Cake.Json"
#addin "nuget:?package=Newtonsoft.Json&version=9.0.1"
#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=coveralls.net&version=0.7.0"
#addin "nuget:?package=Cake.Coveralls"
#addin "nuget:?package=Cake.Docker"

// compile
var compileConfig = ValidateConfig(Argument("configuration", "Release"));
var slnFile = "./src/Evelyn.sln";

// build artifacts
var artifactsDir = Directory("artifacts");

// unit testing
var artifactsForUnitTestsDir = artifactsDir + Directory("UnitTests");
var unitTestAssemblies = new []
{
	@"./src/Evelyn.Management.Api.Rest.Tests/Evelyn.Management.Api.Rest.Tests.csproj",
	@"./src/Evelyn.Core.Tests/Evelyn.Core.Tests.csproj",
	@"./src/Evelyn.Client.Tests/Evelyn.Client.Tests.csproj",
};
var netFrameworkUnitTestAssemblies = new []
{
	@"./src/Evelyn.Storage.EventStore.Tests/bin/"+compileConfig+"/net461/Evelyn.Storage.EventStore.Tests.dll",
};
var openCoverSettings = new OpenCoverSettings();
var minCodeCoverage =85d;
var coverallsRepoToken = "coveralls-repo-token-evelyn";

// integration testing
var artifactsForIntegrationTestsDir = artifactsDir + Directory("IntegrationTests");
var integrationTestAssemblies = new []
{
	@"./src/Evelyn.Management.Api.Rest.IntegrationTests/Evelyn.Management.Api.Rest.IntegrationTests.csproj",
};

// packaging
var packagesDir = artifactsDir + Directory("Packages");
var releaseNotesFile = packagesDir + File("releasenotes.md");
var artifactsFile = packagesDir + File("artifacts.txt");
var packageNames = new []
{
	"Evelyn.Core",
	"Evelyn.Management.Api.Rest",
	"Evelyn.Client",
	"Evelyn.Client.Rest",
	"Evelyn.Storage.EventStore"
};

// unstable releases
var nugetFeedUnstableKey = EnvironmentVariable("nuget-apikey-unstable");
var nugetFeedUnstableUploadUrl = "https://www.myget.org/F/binarymash-unstable/api/v2/package";
var nugetFeedUnstableSymbolsUploadUrl = "https://www.myget.org/F/binarymash-unstable/symbols/api/v2/package";
var nugetFeedUnstableBranchFilter = "^(develop)$|^(PullRequest/)";

// stable releases
var tagsUrl = "https://api.github.com/repos/binarymash/evelyn/releases/tags/";
var nugetFeedStableKey = EnvironmentVariable("nuget-apikey-stable");
var nugetFeedStableUploadUrl = "https://www.nuget.org/api/v2/package";
var nugetFeedStableSymbolsUploadUrl = "https://www.nuget.org/api/v2/package";

// docker compose
var composeFiles = new[]
{
	"./src/docker-compose.yml",
	"./src/docker-compose.override.yml"
};

// internal build variables - don't change these.
var releaseTag = "";
GitVersion versioning = null;
var committedVersion = "0.0.0-dev";
var buildVersion = committedVersion;

var target = Argument("target", "Default");

Information("target is " +target);
Information("Build configuration is " + compileConfig);	

Task("Default")
	.IsDependentOn("Build");

Task("Build")
	.IsDependentOn("RunTests")
	.IsDependentOn("CreatePackages");

Task("BuildAndReleaseUnstable")
	.IsDependentOn("Build")
	.IsDependentOn("ReleasePackagesToUnstableFeed");
	
Task("Clean")
	.Does(() =>
	{
        if (DirectoryExists(artifactsDir))
        {
			Information($"Cleaning {artifactsDir}");
            CleanDirectory(artifactsDir);
        }
		else
		{
			Information($"Creating {artifactsDir}");
            CreateDirectory(artifactsDir);
		}
	});
	
Task("Version")
	.Does(() =>
	{
		versioning = GetNuGetVersionForCommit();
		var nugetVersion = versioning.NuGetVersion;
		Information("SemVer version number: " + nugetVersion);

		if (AppVeyor.IsRunningOnAppVeyor)
		{
			Information("Persisting version number...");
			PersistVersion(committedVersion, nugetVersion);
			buildVersion = nugetVersion;
		}
		else
		{
			Information("We are not running on build server, so we won't persist the version number.");
		}
	});

Task("Compile")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
	.Does(() =>
	{	
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = compileConfig,
		};
		
		DotNetCoreBuild(slnFile, settings);
	});

Task("RunUnitTestsCoverageReport")
	.IsDependentOn("Compile")
	.Does(context =>
	{
        var coverageSummaryFile = artifactsForUnitTestsDir + File("coverage.xml");
        
        EnsureDirectoryExists(artifactsForUnitTestsDir);
        
		if(!IsRunningOnWindows())
		{
			Warning("We are not running on Windows so we can't run test coverage, but we will run the tests.");
			foreach(var testAssembly in unitTestAssemblies)
			{
				DotNetCoreTest(testAssembly, new DotNetCoreTestSettings()
				{
					ArgumentCustomization = args => args
						.Append("--no-build")
						.Append("--no-restore")
						.Append("--results-directory " + artifactsForUnitTestsDir)
						.Append("--configuration " + compileConfig)
				});
			}			
			return;
		} 

		foreach(var testAssembly in unitTestAssemblies)
		{
			Information("Running test task for " + testAssembly);
			
			OpenCover(tool => 
				{
					tool.DotNetCoreTest(testAssembly);
				},
				new FilePath(coverageSummaryFile),
				new OpenCoverSettings()
				{
					Register="user",
					ArgumentCustomization=args=>args.Append(@"-oldstyle -returntargetcode -mergeoutput -mergebyhash")
				}
				.WithFilter("+[Evelyn.*]Evelyn.*")
				.WithFilter("-[xunit*]*")
				.WithFilter("-[Evelyn.*.Tests]*")
				.WithFilter("-[Evelyn.*]*.BackgroundService")
			);
		}
        
		foreach(var testAssembly in netFrameworkUnitTestAssemblies)
		{
			Information("Running test task for " + testAssembly);
			
			if (!IsCrossPlatformConfig(compileConfig))
			{
				XUnit2(testAssembly, new XUnit2Settings 
				{
					Parallelism = ParallelismOption.None,
					ShadowCopy = false,
					XmlReport = true,
					HtmlReport = true,
					OutputDirectory = artifactsForUnitTestsDir
				});
			}
			else
			{
				Warning($"The tests for {testAssembly} can only run on Windows");
			}
		}

		Information($"writing to {artifactsForUnitTestsDir}"); 
        ReportGenerator(coverageSummaryFile, artifactsForUnitTestsDir);
		
		if (IsAuthoritativeBuild())
		{
			var repoToken = EnvironmentVariable(coverallsRepoToken);
			if (string.IsNullOrEmpty(repoToken))
			{
				throw new Exception(string.Format("Coveralls repo token not found. Set environment variable '{0}'", coverallsRepoToken));
			}

			Information("Uploading test coverage to coveralls.io");
			CoverallsNet(coverageSummaryFile, CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
			{
				RepoToken = repoToken
			});
		}
		else
		{
			Information("We are not running on the authoritative build server so we won't publish the coverage report to coveralls.io");
		}

		var sequenceCoverage = XmlPeek(coverageSummaryFile, "//CoverageSession/Summary/@sequenceCoverage");
		var branchCoverage = XmlPeek(coverageSummaryFile, "//CoverageSession/Summary/@branchCoverage");

		Information("Sequence Coverage: " + sequenceCoverage);
		
		if(double.Parse(sequenceCoverage) < minCodeCoverage)
		{
			throw new Exception(string.Format("Code coverage fell below the threshold of {0}%", minCodeCoverage));
		};
	});

Task("RunIntegrationTests")
	.IsDependentOn("Compile")
	.Does(() => 
	{
        EnsureDirectoryExists(artifactsForIntegrationTestsDir);
        
		foreach(var testAssembly in integrationTestAssemblies)
		{
			DotNetCoreTest(testAssembly, new DotNetCoreTestSettings()
			{
				Configuration = compileConfig,
				ArgumentCustomization = args => args
					.Append("--no-build")
					.Append("--no-restore")
					.Append("--results-directory " + artifactsForIntegrationTestsDir)
			});
		}        
	});

Task("RunTests")
	.IsDependentOn("RunUnitTestsCoverageReport")
	.IsDependentOn("RunIntegrationTests");

Task("CreatePackages")
	.IsDependentOn("Compile")
	.Does(() => 
	{
		EnsureDirectoryExists(packagesDir);
        
		CopyFiles("./src/**/Evelyn.*.nupkg", packagesDir);

		GenerateReleaseNotes(releaseNotesFile);

		foreach(var packageName in packageNames)
		{
	        System.IO.File.AppendAllLines(artifactsFile, new[]{$"nuget:{packageName}.{buildVersion}.nupkg"});
		}
	    System.IO.File.AppendAllLines(artifactsFile, new[]{"releaseNotes:releasenotes.md"});

		if (AppVeyor.IsRunningOnAppVeyor)
		{
			var path = packagesDir.ToString() + @"/**/*";

			foreach (var file in GetFiles(path))
			{
				AppVeyor.UploadArtifact(file.FullPath);
			}
		}
	});

Task("ReleasePackagesToUnstableFeed")
	.IsDependentOn("CreatePackages")
	.Does(() =>
	{
		if (ShouldPublishToUnstableFeed(nugetFeedUnstableBranchFilter, versioning.BranchName))
		{
			PublishPackages(packagesDir, artifactsFile, nugetFeedUnstableKey, nugetFeedUnstableUploadUrl, nugetFeedUnstableSymbolsUploadUrl);
		}
	});

Task("EnsureStableReleaseRequirements")
    .Does(() =>
    {
        if (!IsAuthoritativeBuild())
		{
           throw new Exception("Stable release should happen via authoriative build server");
		}
        
		var isTag =
           AppVeyor.Environment.Repository.Tag.IsTag &&
           !string.IsNullOrWhiteSpace(AppVeyor.Environment.Repository.Tag.Name);

        if (!isTag)
		{
           throw new Exception("Stable release should happen from a published GitHub release");
		}
    });

Task("UpdateVersionInfo")
    .IsDependentOn("EnsureStableReleaseRequirements")
    .Does(() =>
    {
        releaseTag = AppVeyor.Environment.Repository.Tag.Name;
        AppVeyor.UpdateBuildVersion(releaseTag);
    });

Task("DownloadGitHubReleaseArtifacts")
    .IsDependentOn("UpdateVersionInfo")
    .Does(() =>
    {
        EnsureDirectoryExists(packagesDir);

		var releaseUrl = tagsUrl + releaseTag;
        var assets_url = Newtonsoft.Json.Linq.JObject.Parse(GetResource(releaseUrl))
            .GetValue("assets_url")
			.Value<string>();

        foreach(var asset in Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetResource(assets_url)))
        {
			var file = packagesDir + File(asset.Value<string>("name"));
			Information("Downloading " + file);
            DownloadFile(asset.Value<string>("browser_download_url"), file);
        }
    });

Task("ReleasePackagesToStableFeed")
    .IsDependentOn("DownloadGitHubReleaseArtifacts")
    .Does(() =>
    {
		PublishPackages(packagesDir, artifactsFile, nugetFeedStableKey, nugetFeedStableUploadUrl, nugetFeedStableSymbolsUploadUrl);
    });

Task("Release")
    .IsDependentOn("ReleasePackagesToStableFeed");

Task("RunSample")
	.Does(() =>
	{
		var settings = new DockerComposeUpSettings()
		{
			Build = true,
			Files = composeFiles,
			ForceRecreate = true
		};
		DockerComposeUp(settings);
	});

RunTarget(target);

private bool IsAuthoritativeBuild()
{
	return AppVeyor.IsRunningOnAppVeyor && IsRunningOnWindows();
}

private string ValidateConfig(string config)
{
	if (IsRunningOnWindows())
	{
		return config;
	}

	if (IsCrossPlatformConfig(config))
	{
		return config;	
	}

	config += "-xPlatform";
	Information("Not running on Windows, so switching build configuration to " + config);

	return config;
}

private bool IsCrossPlatformConfig(string config)
{
	return config.EndsWith("-xPlatform");
}

/// Gets unique nuget version for this commit
private GitVersion GetNuGetVersionForCommit()
{
    GitVersion(new GitVersionSettings{
        UpdateAssemblyInfo = false,
        OutputType = GitVersionOutput.BuildServer
    });

    return GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
}

/// Updates project version in all of our projects
private void PersistVersion(string committedVersion, string newVersion)
{
	Information(string.Format("We'll search all csproj files for {0} and replace with {1}...", committedVersion, newVersion));

	var projectFiles = GetFiles("./**/*.csproj");

	foreach(var projectFile in projectFiles)
	{
		var file = projectFile.ToString();
 
		Information(string.Format("Updating {0}...", file));

		var updatedProjectFile = System.IO.File.ReadAllText(file)
			.Replace(committedVersion, newVersion);

		System.IO.File.WriteAllText(file, updatedProjectFile);
	}
}

/// generates release notes based on issues closed in GitHub since the last release
private void GenerateReleaseNotes(ConvertableFilePath releaseNotesFile)
{
	if(!IsRunningOnWindows())
	{
		Warning("We are not running on Windows so we cannot generate release notes.");
		return;
	}

	try
	{
		Information("Generating release notes at " + releaseNotesFile);

		GitReleaseNotes(releaseNotesFile, new GitReleaseNotesSettings 
		{
			WorkingDirectory = "."
		});

		if (string.IsNullOrEmpty(System.IO.File.ReadAllText(releaseNotesFile)))
		{
			System.IO.File.WriteAllText(releaseNotesFile, "No issues closed since last release");
		}
	} 
	catch(Exception ex)
	{
		Warning("Couldn't create release notes: " + ex);
	}
}

/// Publishes code and symbols packages to nuget feed, based on contents of artifacts file
private void PublishPackages(ConvertableDirectoryPath packagesDir, ConvertableFilePath artifactsFile, string feedApiKey, string codeFeedUrl, string symbolFeedUrl)
{
        var packages = System.IO.File
            .ReadAllLines(artifactsFile)
            .Select(line => line.Split(':'))
			.Where(splitLine => splitLine[0] == "nuget") 
            .Select(splitLine => splitLine[1]);

		foreach(var package in packages)
		{
			var codePackage = packagesDir + File(package);

			NuGetPush(
				codePackage,
				new NuGetPushSettings {
					ApiKey = feedApiKey,
					Source = codeFeedUrl
				});
		}
}

/// gets the resource from the specified url
private string GetResource(string url)
{
	Information("Getting resource from " + url);

    var assetsRequest = System.Net.WebRequest.CreateHttp(url);
    assetsRequest.Method = "GET";
    assetsRequest.Accept = "application/vnd.github.v3+json";
    assetsRequest.UserAgent = "BuildScript";

    using (var assetsResponse = assetsRequest.GetResponse())
    {
        var assetsStream = assetsResponse.GetResponseStream();
        var assetsReader = new StreamReader(assetsStream);
        return assetsReader.ReadToEnd();
    }
}

private bool ShouldPublishToUnstableFeed(string filter, string branchName)
{
	var regex = new System.Text.RegularExpressions.Regex(filter);
	var publish = regex.IsMatch(branchName);
	if (publish)
	{
		Information("Branch " + branchName + " will be published to the unstable feed");
	}
	else
	{
		Information("Branch " + branchName + " will not be published to the unstable feed");
	}
	return publish;	
}