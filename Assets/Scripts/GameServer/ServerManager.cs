using Marsion.Server;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    // Server는 WRGBYK 중 B 계열을 사용합니다.

    public class ServerManager : MonoBehaviour
    {
        [SerializeField] GameServer _gameServer;
        public GameServer GameServer => _gameServer;
        public DraftServer DraftServer;

        public void Init()
        {
            Managers.Logger.Log<ServerManager>("Initialized", colorName: "#FFA500");

            Managers.Network.OnClientJoin -= ClientConnected;
            Managers.Network.OnClientJoin += ClientConnected;

            GameServer.Init();

            DraftServer = new DraftServer();
            DraftServer.Init();
        }

        private void Clear()
        {
            GameServer.Clear();
            Managers.Network.OnClientJoin -= ClientConnected;
        }

        private void ClientConnected(ulong clientID)
        {
            Managers.Logger.Log<ServerManager>($"Client(ID:{clientID}) connected", colorName: "#FFA500");

            if(!Managers.Network.IsHost)
            {
                Clear();
                gameObject.SetActive(false);
                return;
            }

            GameServer.CheckConnectionRpc();
        }
    }
}