namespace Evelyn.Core.WriteModel
{
    using System.Threading.Tasks;

    public interface IStartUpCommands
    {
        Task Execute();
    }
}
