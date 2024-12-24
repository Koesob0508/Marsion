using System.Collections.Generic;

namespace Marsion
{
    public interface IGameSessionFactory
    {
        INetworkManagerEx ProvideNetwork();
        IDataManager ProvideData();
        List<ulong> ProvidePlayersClientIDs();
        IGameLogicFactory CreateGameLogicFactory();
        IGameLogicEx CreateGameLogicEx();
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
        public List<ulong> ProvidePlayersClientIDs()
        {
            var clientIDs = new List<ulong>();

            foreach (var playerInfo in _playerInfos)
            {
                clientIDs.Add(playerInfo.ClientID);
            }

            return clientIDs;
        }
        public IGameLogicFactory CreateGameLogicFactory() => new DefaultGameLogicFactory(_managers, _playerInfos);
        public IGameLogicEx CreateGameLogicEx() => new DefaultGameLogic();
    }
}