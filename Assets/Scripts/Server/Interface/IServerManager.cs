using Marsion.Server;

namespace Marsion
{
    public interface IServerManager
    {
        DraftServer DraftServer { get; }
        GameServerEx GameServer { get; }
        void Init(INetworkManagerEx networkManager, IServerFactory serverFactory);
    }
}