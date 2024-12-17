using System;

namespace Marsion
{
    public interface IServerFactory
    {
        DraftServer CreateDraftServer();
        GameServerEx CreateGameServerEx();
    }

    public class DefaultServerFactory : IServerFactory
    {
        public DraftServer CreateDraftServer() => new DraftServer();
        public GameServerEx CreateGameServerEx()
        {
            var existingGameServer = UnityEngine.Object.FindAnyObjectByType<GameServerEx>();
            if(existingGameServer == null)
            {
                throw new InvalidOperationException("GameServer is not found in the scene. " +
                    "Ensure a GameServer is pre-placed in the scene or properly configured in the environment.");
            }

            return existingGameServer;
        }
    }
}