using Marsion.Server;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    // Server는 WRGBYK 중 B 계열을 사용합니다.

    public class ServerManager : MonoBehaviour, IServerManager
    {
        private INetworkManagerEx _networkManager;
        private IServerFactory _serverFactory;

        public GameServerEx GameServer { get; private set; }
        public DraftServer DraftServer { get; private set; }

        public void Init(INetworkManagerEx networkManager, IServerFactory serverFactory)
        {
            _networkManager = networkManager;
            _serverFactory = serverFactory;

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

            GameServer = _serverFactory.CreateGameServerEx();
            GameServer.Init();

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
            GameServer = null;
            _networkManager.OnClientConnected -= OnClientJoin;
            _networkManager.OnConnect -= OnConnect;
        }
    }
}