namespace Evelyn.Core.ReadModel.Projections
{
    public abstract class ProjectionBuilder<TDto>
    {
        public ProjectionBuilder(IProjectionStore<TDto> projections)
        {
            Projections = projections;
        }

        protected IProjectionStore<TDto> Projections { get; }
    }
}
