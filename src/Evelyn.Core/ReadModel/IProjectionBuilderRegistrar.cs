namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;

    public interface IProjectionBuilderRegistrar
    {
        void Register(Type handler, IEnumerable<IProjectionBuilder> projectionBuilders);

        IEnumerable<IProjectionBuilder> Get(Type handler);
    }
}