namespace Marsion
{
    public interface IServerManager
    {
        DraftServer DraftServer { get; }
        IGameModel GameModel { get; }
        void Init(IServerManagerFactory serverFactory);
    }
}