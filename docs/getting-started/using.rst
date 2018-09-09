Using Evelyn
============

We'll assume that you've got the sample client and server running. Now let's try to do something with it.

Accessing the REST Server
-------------------------

The Evelyn REST API endpoints are specified using `OpenAPI <https://www.openapis.org/>`_ and documented using using `Swagger UI <https://swagger.io/tools/swagger-ui/>`_. By default, the sample server host is configured to allow us to access port 2316, and so we can inspect the API in a browser by navigating to ``http://localhost:2316/swagger/``.

.. image:: ../images/swagger-1.png

If you don't see this, it might be because something else is already using port 2316. If this is the case, you'll need to modify the port forwarding configuration for evelyn-server-host in ``./src/docker-compose.yml`` to specify a different port.

When the server runs for the first time, it will set up a default account for us and add create some sample data to get us started. Lets check it is all set up correctly:

- In Swagger UI, expand the `GET /api/projects` section
- Click the `Try it out` button
- Click the `Execute` button

In Evelyn, a project is a logical collection of feature toggles and environments. The ``/api/projects`` endpoint returns us a list of all the projects on our account. When we click the `Execute` button, Swagger will make a call to this endpoint. The response should look something like this:

.. code-block:: json

   {
      "accountId": "e70fd009-22c4-44e0-ab13-2b6edaf0bbdb",
      "projects": [
         {
            "id": "8f73d020-96c4-407e-8602-74fd4e2ed08b",
            "name": "My First Project"
         }
      ],
      "created": "2018-05-27T15:58:13.6253741+00:00",
      "createdBy": "SystemUser",
      "lastModified": "2018-05-27T15:58:30.7611496+00:00",
      "lastModifiedBy": "SystemUser",
      "version": 1
   }

We can see here that our account ID is ``e70fd009-22c4-44e0-ab13-2b6edaf0bbdb``, and we have a project called ``My First Project`` which has the ID ``8f73d020-96c4-407e-8602-74fd4e2ed08b``.

Now we know the ID of the project, lets now get more details about it:

- Expand the `GET /api/projects/{id}` section
- Click the `Try it out` button
- In the `id` input box, enter the id of the project, ``8f73d020-96c4-407e-8602-74fd4e2ed08b``
- Click the `Execute` button

The response should look something like this:

.. code-block:: json

   {
      "id": "8f73d020-96c4-407e-8602-74fd4e2ed08b",
      "name": "My First Project",
      "environments": [
         {
            "key": "my-first-environment"
         }
      ],
      "toggles": [
         {
            "key": "my-first-toggle",
            "name": "My First Toggle"
         }
      ],
      "created": "2018-05-27T15:58:30.7715006+00:00",
      "createdBy": "SystemUser",
      "lastModified": "2018-05-27T15:58:30.8970043+00:00",
      "lastModifiedBy": "SystemUser",
      "version": 2
   }

We can see that the project has a single environment, ``my-first-environment`` and has one toggle, ``my-first-toggle``.

So far so good. Now lets turn our attention to the client.


Using the Client
----------------

An application that uses the Evelyn client must be configured to connect to the server to retrieve the current toggle states for a particular enviroment and project. 

The sample client host is already configured to get the toggle state for the sample project and environment that was created when we started the server; you can find this configuration in ``.\src\Evelyn.Client.Host\Startup.cs``. Note that in this class we also start a background service, which is used to poll the server for the current state.

Now, take a look in the ``ClassWithToggle`` class. You'll see we're injecting an ``IEvelynClient`` in the constructor. This interface lets us access the toggle states for our chosen environment. We use the  ``GetToggleState`` method on this interface to get the current state of our ``my-first-toggle`` toggle, and then use this to decide which block of code to execute.

Look at the logging output from sample - if you're using Visual Studio this will be in in the Output window, or if you're running on the command line it'll be directly in your shell. You should see something like this...

.. code-block:: text
   
   This code is only called when the toggle is OFF.

It's clear from this that our execution path is currently that specified for when the toggle is turned off. 

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


Now look at the logs again....

.. code-block:: text
   
   This code is only called when the toggle is OFF.
   Toggle state has changed.
   This code is only called when the toggle is ON.

Now, we're going through the other code block! So, in changing the toggle state, we've changed the behaviour of our application.


