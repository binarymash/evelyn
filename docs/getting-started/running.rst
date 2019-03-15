Running Evelyn
==============

The repository contains a sample server host and client application, and configuration for `Docker <https://www.docker.com/>`_ to run these using `EventStore <https://eventstore.org/>`_ as our event store. You can run these on the command line, or in Visual Studio.

Prerequisites
-------------

- `Docker <https://www.docker.com/>`_ is installed on your computer. Note that if you're already running in a virtualised environment - for example, Windows running in Parallels on a Mac, then you probably can't use the docker files as Docker doesn't play nicely with nested virtualisation.
- .NET Core 2.2 SDK
- if you're running on linux or macOS the evelyn build scripts require you to have mono 5.12.0 or later installed

Running in Docker using the command line
----------------------------------------

Run the ``./runSample.ps1`` script (Windows) or ``./runSample.sh`` (Linux/macOS). This will kick off Cake scripts which will build and then run the Docker containers.

Running in Docker using Visual Studio (Windows)
-----------------------------------------------
Ensure that the startup project is ``docker-compose``, then run the solution.