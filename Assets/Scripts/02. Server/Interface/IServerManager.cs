namespace Marsion
{
    public interface IServerManager
    {
        DraftServer DraftServer { get; }
        IGameSession GameModel { get; }
        void Init(IServerManagerFactory serverFactory);
    }
}