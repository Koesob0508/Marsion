namespace Marsion
{
    public interface ICommandHandler
    {
        void Add(ICommand command);
        void Remove(ICommand command);
        void Clear();
    }
}