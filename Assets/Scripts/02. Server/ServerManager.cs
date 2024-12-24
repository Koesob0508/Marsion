using UnityEngine;

namespace Marsion
{
    public class ServerManager : MonoBehaviour, IServerManager
    {
        private IServerManagerFactory _serverFactory;
        private INetworkManagerEx _networkManager;

        public IGameSession GameSession { get; private set; }
        public DraftServer DraftServer { get; private set; }

        public void Init(IServerManagerFactory serverFactory)
        {
            _serverFactory = serverFactory;
            _networkManager = serverFactory.ProvideNetwork();

            Logger.Log<ServerManager>("Server Manager initialized", colorName: ColorCodes.Server);

            _networkManager.OnConnect += OnConnect;
            _networkManager.OnClientConnected += OnClientJoin;
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

            DraftServer = _serverFactory.CreateDraftServer();
            DraftServer.Init();

            GameSession = _serverFactory.CreateGameSession();
            IGameSessionFactory gameFactory = _serverFactory.CreateGameFactory(_serverFactory.ProvideManagers()); 
            GameSession.Init(gameFactory);

            RegisterClient(_networkManager.LocalID);
        }

        private void OnClientJoin(ulong clientID)
        {
            if(clientID != _networkManager.ServerID)
            {
                Logger.Log<ServerManager>("OnClientJoin");
                RegisterClient(clientID);
            }
        }

        private void RegisterClient(ulong clientID)
        {
            Logger.Log<ServerManager>($"Client(ID : {clientID}) regist", colorName: ColorCodes.Server);
            DraftServer.AddState(clientID);
        }

        private void Clear()
        {
            Logger.Log<ServerManager>("Server Manager cleared", colorName: ColorCodes.Server);
            DraftServer = null;
            GameSession = null;
            _networkManager.OnClientConnected -= OnClientJoin;
            _networkManager.OnConnect -= OnConnect;
        }
    }
}