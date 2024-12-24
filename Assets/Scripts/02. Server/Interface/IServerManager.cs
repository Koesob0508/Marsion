namespace Marsion
{
    public interface IServerManager
    {
        DraftServer DraftServer { get; }
        IGameSession GameSession { get; }
        void Init(IServerManagerFactory serverFactory);
        void Ready(ulong clientID, DraftState state);
    }
}