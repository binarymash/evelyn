Building the code
=================

Evelyn can be built on the command line on any environment that supports the .NET Core SDK and, if you're running on Windows, in any version of Visual Studio 2017.

Prerequisites
-------------

- .NET Core 2.2 SDK
- if you're running on linux or MacOS the evelyn build scripts require you to have mono 5.12.0 or later installed


Building on the command line
----------------------------

This project has automated build scripts that will compile the code and run the test suite. The build scripts are written using `Cake <http://cakebuild.net/>`_. These scripts can be used by developers, and are also used in the project's build and release pipeline in AppVeyor.

* Ensure that you have the latest .NET Core SDK installed

* Run the appropriate build script for your development environment

   * ``./build.ps1`` (Powershell)
   * ``./build.sh`` (Linux/macOS shell)

The build will take a few moments. The outputs of the build (nuget packages, test results etc) will be published to the ``./artifacts`` folder. You might need to have administrator rights to run some of the tests.


Building in Visual Studio
-------------------------

- Open Visual Studio 2017 with administrator rights (you'll need this to run some of the tests, and when running the server in debug)
- Open the ``./src/Evelyn.sln`` solution
- Build the solution