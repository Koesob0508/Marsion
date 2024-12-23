namespace Marsion
{
    public interface IGameModelFactory
    {
        INetworkManagerEx ProvideNetwork();
        IDataManager ProvideData();
        IGameData CreateGameData();
        IGameDataHandler CreateGameDataHandler(IGameData gameData);
        IGameLogicEx CreateGameLogicEx(IGameDataHandler gameDataHandler);
    }

    public class DefaultGameModelFactory : IGameModelFactory
    {
        private IManagers _managers;

        public DefaultGameModelFactory(IManagers managers)
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