namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using ProjectList;
    using AccountEvents = WriteModel.Account.Events;
    using ProjectEvents = WriteModel.Project.Events;

    public class AccountProjectsHandler
        : ICancellableEventHandler<AccountEvents.AccountRegistered>,
        ICancellableEventHandler<ProjectEvents.ProjectCreated>
    {
        private readonly IDatabase<Guid, AccountProjectsDto> _db;

        public AccountProjectsHandler(IDatabase<Guid, AccountProjectsDto> db)
        {
            _db = db;
        }

        public async Task Handle(AccountEvents.AccountRegistered message, CancellationToken token = default(CancellationToken))
        {
            var accountProjects = new AccountProjectsDto(message.Id);
            await _db.AddOrUpdate(accountProjects.AccountId, accountProjects);
        }

        public async Task Handle(ProjectEvents.ProjectCreated message, CancellationToken token)
        {
            var accountProjects = await _db.Get(message.AccountId);
            accountProjects.Projects.Add(message.Id, new ProjectListDto(message.Id, message.Name));
            await _db.AddOrUpdate(accountProjects.AccountId, accountProjects);
        }
    }
}
