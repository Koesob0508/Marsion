using System.Collections.Generic;

namespace Marsion
{
    public interface IGameDataHandlerFactory
    {
        IGameLogic ProvideGameLogic();
        IGameLogicConfig ProvideGameLogicConfig();
        List<PlayerInfo> ProvidePlayerInfos();
        IGameData CreateGameData();
    }

    public class DefaultGameDataHandlerFactory : IGameDataHandlerFactory
    {
        private IGameLogic _gameLogic;
        private IGameLogicConfig _logicConfig;
        private List<PlayerInfo> _playerInfos;

        public DefaultGameDataHandlerFactory(IGameLogic gameLogic, IGameLogicConfig logicConfig, List<PlayerInfo> playerInfos)
        {
            _gameLogic = gameLogic;
            _logicConfig = logicConfig;
            _playerInfos = playerInfos;
        }

        public IGameLogic ProvideGameLogic() => _gameLogic;
        public IGameLogicConfig ProvideGameLogicConfig() => _logicConfig;
        public List<PlayerInfo> ProvidePlayerInfos() => _playerInfos;
        public IGameData CreateGameData() => new DefaultGameData();
    }
}