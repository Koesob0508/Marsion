using Marsion.Server;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    // Server는 WRGBYK 중 B 계열을 사용합니다.

    public class ServerManager : MonoBehaviour
    {
        public GameServer Game;
        public GameServerEx GameEx;
        public DraftServer Draft;

        public void Init()
        {
            Logger.Log<ServerManager>("Server Manager initialized", colorName: ColorCodes.Server);

            Managers.Instance.Network.OnConnect += OnConnect;
            Managers.Instance.Network.OnClientConnected += OnClientJoin;
        }

        private void OnConnect()
        {
            if (!Managers.Instance.Network.IsHost)
            {
                Logger.Log<ServerManager>("This is not host client", colorName: ColorCodes.Server);

                Clear();
                gameObject.SetActive(false);
                return;
            }

            Logger.Log<ServerManager>("OnConnect");

            Draft = new DraftServer();
            Draft.Init();

            //Game.Init();
            GameEx.Init();

            RegisterClient(Managers.Instance.Network.LocalID);
        }

        private void Clear()
        {
            Logger.Log<ServerManager>("Server Manager cleared", colorName: ColorCodes.Server);

            Game.Clear();
            Managers.Instance.Network.OnClientConnected -= OnClientJoin;
        }

        private void OnClientJoin(ulong clientID)
        {
            if(clientID != Managers.Instance.Network.ServerID)
            {
                Logger.Log<ServerManager>("OnClientJoin");
                RegisterClient(clientID);
            }
        }

        private void RegisterClient(ulong clientID)
        {
            Logger.Log<ServerManager>($"Client(ID : {clientID}) regist", colorName: ColorCodes.Server);

            Draft.AddState(clientID);
        }
    }
}