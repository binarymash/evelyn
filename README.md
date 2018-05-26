# Evelyn

[![license](https://img.shields.io/github/license/binarymash/evelyn.svg)](https://github.com/binarymash/evelyn/blob/develop/LICENSE)  [![Build status](https://ci.appveyor.com/api/projects/status/fe6ta6qtgyat5i6u/branch/develop?svg=true)](https://ci.appveyor.com/project/binarymash/evelyn/branch/develop) [![Coverage Status](https://coveralls.io/repos/github/binarymash/evelyn/badge.svg?branch=develop)](https://coveralls.io/github/binarymash/evelyn?branch=develop)


Evelyn is a [feature toggling](https://martinfowler.com/articles/feature-toggles.html) framework written in .NET Core. It allows users to decouple software releases from the functional changes within, reducing the risk of deployment and providing rollback functionality. 

The Evelyn Stack consists of the following parts:

- A core framework which provides the underlying feature toggling functionality

- A REsT API server and client that expose this functionality over HTTP

- A management user interface (see [https://github.com/binarymash/evelyn-management-ui](https://github.com/binarymash/evelyn-management-ui))

Evelyn has a modular architecture which allows for flexible deployment configurations and user extensibility. The core framework is built around CQRS and Event Sourcing: implementations are provided for an in-memory event store and for Greg Young's [Event Store](https://eventstore.org/); you can plug in your own event store imtegration.

This project is pre-release: things might break at any moment; APIs might change; it is insecure. 

For more information [Read the Docs](https://evelyn.readthedocs.io/en/latest/)


## Build Status

The package is built on [AppVeyor](https://ci.appveyor.com/project/binarymash/evelyn).

### Stable

[![Build status](https://ci.appveyor.com/api/projects/status/fe6ta6qtgyat5i6u/branch/master?svg=true)](https://ci.appveyor.com/project/binarymash/evelyn/branch/master)

[![Coverage Status](https://coveralls.io/repos/github/binarymash/evelyn/badge.svg?branch=master)](https://coveralls.io/github/binarymash/evelyn?branch=master)

Development builds are published to https://www.myget.org/F/binarymash-stable/api/v3/index.json

### Development

[![Build status](https://ci.appveyor.com/api/projects/status/fe6ta6qtgyat5i6u/branch/develop?svg=true)](https://ci.appveyor.com/project/binarymash/evelyn/branch/develop)

[![Coverage Status](https://coveralls.io/repos/github/binarymash/evelyn/badge.svg?branch=develop)](https://coveralls.io/github/binarymash/evelyn?branch=develop)

Development builds are published to https://www.myget.org/F/binarymash-unstable/api/v3/index.json
