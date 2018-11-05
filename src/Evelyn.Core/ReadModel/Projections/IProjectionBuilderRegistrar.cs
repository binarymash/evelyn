namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using System.Collections.Generic;

    public interface IProjectionBuilderRegistrar
    {
        void Register(Type eventStreamHandlerType, IEnumerable<Type> projectionBuilderTypes);

        ProjectionBuildersByEventType Get(Type eventStreamHandlerType);
    }
}