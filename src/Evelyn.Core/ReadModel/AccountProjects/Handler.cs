namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using AccountEvents = WriteModel.Account.Events;

    public class Handler
        : ICancellableEventHandler<AccountEvents.AccountRegistered>,
        ICancellableEventHandler<AccountEvents.ProjectCreated>
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, AccountProjectsDto> _projectionBuilder;
        private readonly IDatabase<Guid, AccountProjectsDto> _db;

        public Handler(IProjectionBuilder<ProjectionBuilderRequest, AccountProjectsDto> projectionBuilder, IDatabase<Guid, AccountProjectsDto> db)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
        }

        public async Task Handle(AccountEvents.AccountRegistered message, CancellationToken token = default(CancellationToken))
        {
            await UpdateProjection(message.Id, token);
        }

        public async Task Handle(AccountEvents.ProjectCreated message, CancellationToken token)
        {
            await UpdateProjection(message.Id, token);
        }

        private async Task UpdateProjection(Guid id, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(new ProjectionBuilderRequest(id), token);
            await _db.AddOrUpdate(id, dto);
        }
    }
}
