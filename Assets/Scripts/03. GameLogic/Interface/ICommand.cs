namespace Marsion
{
    public interface ICommand
    {
        void Execute(IGameDataHandler dataHandler);
    }
}