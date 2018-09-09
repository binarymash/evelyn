Running Evelyn
==============

The repository contains a sample server host and client application, and configuration for `Docker <https://www.docker.com/>`_ to run these using `EventStore <https://eventstore.org/>`_ as our event store. You can run these on the command line, or in Visual Studio.

Prerequisites
-------------

- `Docker <https://www.docker.com/>`_ is installed on your computer. Note that if you're already running in a virtualised environment - for example, Windows running in Parallels on a Mac, then you probably can't use the docker files as Docker doesn't play nicely with nested virtualisation.

Running in Docker using the command line
----------------------------------------

Run the ``./runSample.ps1`` script. This will kick off Cake scripts which will build and then run the Docker containers.

Running in Docker using Visual Studio
-------------------------------------
Ensure that the startup project is ``docker-compose``, then run the solution.