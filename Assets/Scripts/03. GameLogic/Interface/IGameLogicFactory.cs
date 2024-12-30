using System.Collections.Generic;

namespace Marsion
{
    public interface IGameLogicFactory
    {
        // Managers
        IDataManager ProvideDataManager();

        List<PlayerInfo> ProvidePlayerInfo();
        IGameLogicConfig CreateGameLogicConfig();
        IGameDataHandlerFactory CreateGameDataHandlerFactory(IGameLogic gameLogic, IGameLogicConfig logicConfig);
        IGameDataHandler CreateGameDataHandler();
    }

    public class DefaultGameLogicFactory : IGameLogicFactory
    {
        private IManagers _managers;
        private List<PlayerInfo> _playerInfos;

        public DefaultGameLogicFactory(IManagers managers, List<PlayerInfo> playerInfos)
        {
            _managers = managers;
            _playerInfos = playerInfos;
        }

        public IDataManager ProvideDataManager() => _managers.Data;

        public List<PlayerInfo> ProvidePlayerInfo() => _playerInfos;
        public IGameLogicConfig CreateGameLogicConfig() => new DefaultGameLogicConfig();
        public IGameDataHandlerFactory CreateGameDataHandlerFactory(IGameLogic gameLogic, IGameLogicConfig logicConfig) => new DefaultGameDataHandlerFactory(gameLogic, logicConfig, _playerInfos);
        public IGameDataHandler CreateGameDataHandler() => new DefaultGameDataHandler();
    }
}