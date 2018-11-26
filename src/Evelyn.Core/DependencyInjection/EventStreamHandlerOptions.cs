// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    public class EventStreamHandlerOptions
    {
        public EventStreamHandlerOptions()
        {
            ProjectionBuilders = new List<Type>();
        }

        // TODO: restrict to IProjectionBuilder
        public List<Type> ProjectionBuilders { get; }
    }
}