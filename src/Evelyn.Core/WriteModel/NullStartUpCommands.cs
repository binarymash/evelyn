namespace Evelyn.Core.WriteModel
{
    using System.Threading.Tasks;

    public class NullStartUpCommands : IStartUpCommands
    {
        public Task Execute()
        {
            return Task.CompletedTask;
        }
    }
}
