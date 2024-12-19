using UnityEngine;

namespace Marsion
{
    public class ServerManager : MonoBehaviour, IServerManager
    {
        private IServerManagerFactory _serverFactory;
        private INetworkManagerEx _networkManager;

        public IGameModel GameModel { get; private set; }
        public DraftServer DraftServer { get; private set; }

        public void Init(IServerManagerFactory serverFactory)
        {
            _serverFactory = serverFactory;
            _networkManager = serverFactory.CreateNetworkManager();

            Logger.Log<ServerManager>("Server Manager initialized", colorName: ColorCodes.Server);

            Managers.Instance.Network.OnConnect += OnConnect;
            Managers.Instance.Network.OnClientConnected += OnClientJoin;
        }

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

            GameModel = _serverFactory.CreateGameModel();
            IGameModelFactory gameFactory = _serverFactory.CreateGameFactory(_networkManager); 
            GameModel.Init(gameFactory);

            RegisterClient(Managers.Instance.Network.LocalID);
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
            GameModel = null;
            _networkManager.OnClientConnected -= OnClientJoin;
            _networkManager.OnConnect -= OnConnect;
        }
    }
}