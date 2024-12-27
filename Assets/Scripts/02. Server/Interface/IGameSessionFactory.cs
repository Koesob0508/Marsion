using System.Collections.Generic;

namespace Marsion
{
    public interface IGameSessionFactory
    {
        INetworkManagerEx ProvideNetwork();
        IDataManager ProvideData();
        Dictionary<ulong, ushort> ProvidePlayersClientIDs();
        IGameLogicFactory CreateGameLogicFactory();
        IGameLogic CreateGameLogicEx();
    }

    public class DefaultGameSessionFactory : IGameSessionFactory
    {
        private IManagers _managers;
        private List<PlayerInfo> _playerInfos;

        public DefaultGameSessionFactory(IManagers managers, List<PlayerInfo> playerInfos)
        {
            _managers = managers;
            _playerInfos = playerInfos;
        }

        public INetworkManagerEx ProvideNetwork() => _managers.Network;
        public IDataManager ProvideData() => _managers.Data; // 함수 네이밍이 좀
        public Dictionary<ulong, ushort> ProvidePlayersClientIDs()
        {
            var clientPlayerIdMap = new Dictionary<ulong, ushort>();

            for (ushort index = 0; index < _playerInfos.Count; index++)
            {
                clientPlayerIdMap.Add(_playerInfos[index].ClientID, index);
            }

            return clientPlayerIdMap;
        }
        public IGameLogicFactory CreateGameLogicFactory() => new DefaultGameLogicFactory(_managers, _playerInfos);
        public IGameLogic CreateGameLogicEx() => new DefaultGameLogic();
    }
}