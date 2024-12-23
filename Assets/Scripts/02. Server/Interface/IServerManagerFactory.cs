using System;

namespace Marsion
{
    public interface IServerManagerFactory
    {
        IManagers ProvideManagers();
        INetworkManagerEx ProvideNetwork();
        DraftServer CreateDraftServer();
        IGameSession CreateGameSession();
        IGameSessionFactory CreateGameFactory(IManagers managers);
    }

    public class DefaultServerFactory : IServerManagerFactory
    {
        private IManagers _managers;

        public DefaultServerFactory(IManagers managers)
        {
            _managers = managers;
        }

        public IManagers ProvideManagers() => _managers;
        public INetworkManagerEx ProvideNetwork() => _managers.Network;
        public DraftServer CreateDraftServer() => new DraftServer();
        public IGameSession CreateGameSession()
        {
            var existingGameServer = UnityEngine.Object.FindAnyObjectByType<DefaultGameSession>();
            if(existingGameServer == null)
            {
                throw new InvalidOperationException("GameServer is not found in the scene. " +
                    "Ensure a GameServer is pre-placed in the scene or properly configured in the environment.");
            }

            return existingGameServer;
        }
        public IGameSessionFactory CreateGameFactory(IManagers managers) => new DefaultGameSessionFactory(managers);
    }
}