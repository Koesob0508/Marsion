namespace Marsion
{
    public interface IGameSessionFactory
    {
        INetworkManagerEx ProvideNetwork();
        IDataManager ProvideData();
        IGameData CreateGameData();
        IGameDataHandler CreateGameDataHandler(IGameData gameData);
        IGameLogicEx CreateGameLogicEx(IGameDataHandler gameDataHandler);
    }

    public class DefaultGameSessionFactory : IGameSessionFactory
    {
        private IManagers _managers;

        public DefaultGameSessionFactory(IManagers managers)
        {
            _managers = managers;
        }

        public INetworkManagerEx ProvideNetwork() => _managers.Network;
        public IDataManager ProvideData() => _managers.Data; // 함수 네이밍이 좀
        public IGameData CreateGameData() => new DefaultGameData();
        public IGameDataHandler CreateGameDataHandler(IGameData gameData) => new DefaultGameDataHandler(gameData);
        public IGameLogicEx CreateGameLogicEx(IGameDataHandler gameDataHandler) => new DefaultGameLogic(gameDataHandler);
    }
}