using System;

namespace Marsion
{
    public interface IServerManagerFactory
    {
        INetworkManagerEx CreateNetworkManager();
        DraftServer CreateDraftServer();
        IGameModel CreateGameModel();
        IGameModelFactory CreateGameFactory(INetworkManagerEx networkManager);
    }

    public class DefaultServerFactory : IServerManagerFactory
    {
        private INetworkManagerEx _networkManager;

        public DefaultServerFactory(INetworkManagerEx networkManager)
        {
            _networkManager = networkManager;
        }

        public INetworkManagerEx CreateNetworkManager() => _networkManager;
        public DraftServer CreateDraftServer() => new DraftServer();
        public IGameModel CreateGameModel()
        {
            var existingGameServer = UnityEngine.Object.FindAnyObjectByType<DefaultGameModel>();
            if(existingGameServer == null)
            {
                throw new InvalidOperationException("GameServer is not found in the scene. " +
                    "Ensure a GameServer is pre-placed in the scene or properly configured in the environment.");
            }

            return existingGameServer;
        }
        public IGameModelFactory CreateGameFactory(INetworkManagerEx networkManager) => new DefaultGameModelFactory(networkManager);
    }
}