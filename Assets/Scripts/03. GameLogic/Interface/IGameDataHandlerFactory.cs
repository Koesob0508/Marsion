using System.Collections.Generic;

namespace Marsion
{
    public interface IGameDataHandlerFactory
    {
        IGameLogicConfig ProvideGameLogicConfig();
        List<PlayerInfo> ProvidePlayerInfos();
        IGameData CreateGameData();
    }

    public class DefaultGameDataHandlerFactory : IGameDataHandlerFactory
    {
        private IManagers _managers;
        private IGameLogicConfig _logicConfig;
        private List<PlayerInfo> _playerInfos;

        public DefaultGameDataHandlerFactory(IManagers managers, IGameLogicConfig logicConfig, List<PlayerInfo> playerInfos)
        {
            _managers = managers;
            _logicConfig = logicConfig;
            _playerInfos = playerInfos;
        }

        public IGameLogicConfig ProvideGameLogicConfig() => _logicConfig;
        public List<PlayerInfo> ProvidePlayerInfos() => _playerInfos;
        public IGameData CreateGameData() => new DefaultGameData();
    }
}