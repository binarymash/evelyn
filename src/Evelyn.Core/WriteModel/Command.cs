namespace Evelyn.Core.WriteModel
{
    using CQRSlite.Commands;

    public abstract class Command : ICommand
    {
        protected Command(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}
