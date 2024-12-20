namespace Marsion
{
    public interface IGameModelFactory
    {
        INetworkManagerEx CreateNetworkManager();
        IGameData CreateGameData();
        IGameDataHandler CreateGameDataHandler(IGameData gameData);
        IGameLogicEx CreateGameLogicEx(IGameDataHandler gameDataHandler);
    }

    public class DefaultGameModelFactory : IGameModelFactory
    {
        private INetworkManagerEx _networkManager;

        public DefaultGameModelFactory(INetworkManagerEx networkManager)
        {
            _networkManager = networkManager;
        }

        public INetworkManagerEx CreateNetworkManager() => _networkManager;
        public IGameData CreateGameData() => new DefaultGameData();
        public IGameDataHandler CreateGameDataHandler(IGameData gameData) => new DefaultGameDataHandler(gameData);
        public IGameLogicEx CreateGameLogicEx(IGameDataHandler gameDataHandler) => new DefaultGameLogic(gameDataHandler);
    }
}