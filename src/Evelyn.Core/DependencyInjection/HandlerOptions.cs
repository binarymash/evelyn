// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    public class HandlerOptions
    {
        public HandlerOptions()
        {
            Handlers = new List<Type>();
        }

        public List<Type> Handlers { get; }
    }
}