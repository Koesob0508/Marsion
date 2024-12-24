using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marsion
{
    public class ServerManager : MonoBehaviour, IServerManager
    {
        private Queue<PlayerInfo> _readyPlayers;
        private Dictionary<ulong, PlayerInfo> _playingPlayers;

        private IServerManagerFactory _serverFactory;
        private INetworkManagerEx _networkManager;

        public IGameSession GameSession { get; private set; }
        public DraftServer DraftServer { get; private set; }

        public void Init(IServerManagerFactory serverFactory)
        {
            _readyPlayers = new();
            _playingPlayers = new();

            _serverFactory = serverFactory;
            _networkManager = serverFactory.ProvideNetwork();

            Logger.Log<ServerManager>("Server Manager initialized", colorName: ColorCodes.Server);

            _networkManager.OnConnect += OnConnect;
            _networkManager.OnOtherClientJoin += OnOtherClientJoin;
        }

        /// <summary>
        ///     Draft가 진행되지 않았다면 Draft를 매치
        ///     Draft 완료 후, Ready까지 진행했다면, 따로 관리
        ///     Ready Player가 두 명이라면 OpenSession
        /// </summary>
        private void OnConnect()
        {
            if (!_networkManager.IsHost)
            {
                Logger.Log<ServerManager>("This is not host client", colorName: ColorCodes.Server);

                Clear();
                gameObject.SetActive(false);
                return;
            }

            Logger.Log<ServerManager>("OnConnect");

            // 원래라면 Draft 진행 상태 보면서 진행해야함
            // 다만 지금은 무조건 Draft를 새로 진행하도록
            DraftServer = _serverFactory.CreateDraftServer();
            DraftServer.Init();
            DraftServer.AddState(_networkManager.LocalID);
        }

        private void OnOtherClientJoin(ulong clientID)
        {
            Logger.Log<ServerManager>("OnClientJoin");
            DraftServer.AddState(clientID);
        }

        public void Ready(ulong clientID, DraftState state)
        {
            // 우선, NetworkManager를 통해서 clientID가 connected인지 검증을 한다.
            if (!_networkManager.ConnectedClientsIDs.Contains(clientID)) return;

            // 존재한다면, DraftState로 PlayerInfo를 구성한다.
            var playerInfo = new PlayerInfo();
            playerInfo.ClientID = clientID;
            playerInfo.Portrait = state.Portrait;
            playerInfo.Deck = state.CurrentDeck;

            // 그 후, _readyPlayer로 넣어준다.
            _readyPlayers.Enqueue(playerInfo);

            // Ready Count가 2가 넘는다? 매칭을 한 후, OpenSession을 진행시킨다.
            if(_readyPlayers.Count >= 2)
            {
                var player1 = _readyPlayers.Dequeue();
                var player2 = _readyPlayers.Dequeue();

                List<PlayerInfo> playerInfos = new List<PlayerInfo> { player1, player2 };

                OpenSession(playerInfos);

                // 두 플레이어는 Playing으로 등록
                _playingPlayers.Add(player1.ClientID, player1);
                _playingPlayers.Add(player2.ClientID, player2);
            }
        }

        private void OpenSession(List<PlayerInfo> playerInfos)
        {
            GameSession = _serverFactory.CreateGameSession();
            IGameSessionFactory gameFactory = _serverFactory.CreateGameSessionFactory(_serverFactory.ProvideManagers(), playerInfos);
            GameSession.Init(gameFactory);
        }

        private void Clear()
        {
            Logger.Log<ServerManager>("Server Manager cleared", colorName: ColorCodes.Server);
            DraftServer = null;
            GameSession = null;
            _networkManager.OnOtherClientJoin -= OnOtherClientJoin;
            _networkManager.OnConnect -= OnConnect;
        }
    }

    public class PlayerInfo
    {
        public ulong ClientID;
        public string Portrait;
        public List<string> Deck;
    }
}