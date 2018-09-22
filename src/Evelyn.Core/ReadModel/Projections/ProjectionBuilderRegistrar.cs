namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using System.Collections.Generic;

    public class ProjectionBuilderRegistrar : IProjectionBuilderRegistrar
    {
        private readonly Dictionary<Type, List<IProjectionBuilder>> _projectionBuilders;

        public ProjectionBuilderRegistrar()
        {
            _projectionBuilders = new Dictionary<Type, List<IProjectionBuilder>>();
        }

        public void Register(Type handler, IEnumerable<IProjectionBuilder> projectionBuilders)
        {
            List<IProjectionBuilder> projectionBuildersForHandler;

            if (!_projectionBuilders.TryGetValue(handler, out projectionBuildersForHandler))
            {
                projectionBuildersForHandler = new List<IProjectionBuilder>();
                _projectionBuilders.Add(handler, projectionBuildersForHandler);
            }

            projectionBuildersForHandler.AddRange(projectionBuilders);
        }

        public IEnumerable<IProjectionBuilder> Get(Type handler)
        {
            return _projectionBuilders[handler];
        }
    }
}
