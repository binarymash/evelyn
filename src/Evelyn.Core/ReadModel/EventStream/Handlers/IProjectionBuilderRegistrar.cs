namespace Evelyn.Core.ReadModel.EventStream.Handlers
{
    using System;
    using System.Collections.Generic;

    public interface IProjectionBuilderRegistrar
    {
        void Register(Type eventStreamHandlerType, IEnumerable<Type> projectionBuilderTypes);

        ProjectionBuildersByEventType Get(Type eventStreamHandlerType);
    }
}