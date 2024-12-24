using System.Collections.Generic;

namespace Marsion
{
    public interface IGameLogicFactory
    {
        List<PlayerInfo> ProvidePlayerInfo();
        IGameLogicConfig CreateGameLogicConfig();
        IGameDataHandlerFactory CreateGameDataHandlerFactory(IGameLogicConfig logicConfig);
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

        public List<PlayerInfo> ProvidePlayerInfo() => _playerInfos;
        public IGameLogicConfig CreateGameLogicConfig() => new DefaultGameLogicConfig();
        public IGameDataHandlerFactory CreateGameDataHandlerFactory(IGameLogicConfig logicConfig) => new DefaultGameDataHandlerFactory(_managers, logicConfig, _playerInfos);
        public IGameDataHandler CreateGameDataHandler() => new DefaultGameDataHandler();
    }
}