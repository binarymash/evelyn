namespace Evelyn.Core.ReadModel.ProjectList
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.AccountProjects;
    using Events;
    using Infrastructure;

    public class AccountProjectsHandler
        : ICancellableEventHandler<ProjectCreated>
    {
        private readonly IDatabase<string, AccountProjectsDto> _db;

        public AccountProjectsHandler(IDatabase<string, AccountProjectsDto> db)
        {
            _db = db;
        }

        public async Task Handle(ProjectCreated message, CancellationToken token)
        {
            AccountProjectsDto accountProjects;
            try
            {
                accountProjects = await _db.Get(message.AccountId);
            }
            catch (NotFoundException)
            {
                accountProjects = new AccountProjectsDto(message.AccountId);
            }

            accountProjects.Projects.Add(message.Id, new ProjectListDto(message.Id, message.Name));
            await _db.AddOrUpdate(accountProjects.AccountId, accountProjects);
        }
    }
}
