Running the client
==================

A sample client application is included - this can be found at ``./src/Evelyn.Client.Host``. This client is already configured to point at the default location of the sample server host, and 

You can run the sample host in Visual Studio or on the command line.

Running in Visual Studio
------------------------

Run the ``Samples\Evelyn.Client.Host`` project

Running on the command line
---------------------------

``dotnet .\src\Evelyn.Client.Host\bin\Release\netcoreapp2.1\evelyn.client.host.dll``

Using a toggle
--------------

An application that uses the Evelyn client must be configured to connect to the server to retrieve the current toggle states for a particular enviroment and project. 

The sample client host is already configured to get the toggle state for the sample project and environment that was created when we started the server; you can find this configuration in ``.\src\Evelyn.Client.Host\Startup.cs``. Note that in this class we also start a background service, which is used to poll the server for the current state.

Now, take a look in the ``OutputWriter`` class. You'll see we're injecting an ``IEvelynClient`` in the constructor. This interface lets us access the toggle states for our chosen environment. We use the  ``GetToggleState`` method on this interface to get the current state of our ``my-first-toggle`` toggle, and then use this to decide which block of code to execute.

Lets run the client application. We'll get this output:

.. code-block:: text
   
   Hit enter to continue, or any other key to exit

Hit enter a few times...

.. code-block:: text
   
   This code is called when the toggle is OFF
   This code is called when the toggle is OFF
   This code is called when the toggle is OFF
   This code is called when the toggle is OFF
   This code is called when the toggle is OFF

It's clear from this that our execution path is that specified for when the toggle is turned off. 

Changing toggle state
---------------------

Now, lets change the state of our toggle. We can do this either through the Swagger UI or via the Evelyn Management UI (if you've set it up):

Changing toggle state in Swagger UI
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

- Expand the `POST /api/projects/{projectId}/environments/{environmentKey}/toggles/{toggleKey}/change-state` section
- Click the `Try it out` button
- In the `projectId` input box, enter the id of the project, ``8f73d020-96c4-407e-8602-74fd4e2ed08b``
- In the `environmentKey` input box, enter the key of our environment, ``my-first-environment``
- In the ``toggleKey`` input box, enter the key of our toggle, ``my-first-toggle``
- In the `message Body` input box, enter this:
.. code-block:: json

   {
     "expectedToggleStateVersion": 0,
     "state": "True"
   }
- Click the `Execute` button

Changing toggle state in Evelyn Management UI
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

- From the dashboard, select `My First Project`
- Select `my-first-environment` from the list of environments
- Find `my-first-toggle` in the list of toggles, and click its icon to change the state from ``OFF`` to ``ON``


Return to the client application and hit enter a few more times...

.. code-block:: text
   
   This code is called when the toggle is ON
   This code is called when the toggle is ON
   This code is called when the toggle is ON
   This code is called when the toggle is ON
   This code is called when the toggle is on

Now, we're going through the other code block! So, in changing the toggle state, we've changed the behaviour of our application.


