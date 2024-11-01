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
            Managers.Logger.Log<ServerManager>("Server Manager initialized", colorName: ColorCodes.Server);

            Managers.Network.OnConnect += OnConnect;
            Managers.Network.OnClientJoin += OnClientJoin;
        }

        private void OnConnect()
        {
            if (!Managers.Network.IsHost)
            {
                Managers.Logger.Log<ServerManager>("This is not host client", colorName: ColorCodes.Server);

                Clear();
                gameObject.SetActive(false);
                return;
            }

            Draft = new DraftServer();
            Draft.Init();

            //Game.Init();
            GameEx.Init();

            RegisterClient(Managers.Network.ClientID);
        }

        private void Clear()
        {
            Managers.Logger.Log<ServerManager>("Server Manager cleared", colorName: ColorCodes.Server);

            Game.Clear();
            Managers.Network.OnClientJoin -= OnClientJoin;
        }

        private void OnClientJoin(ulong clientID)
        {
            RegisterClient(clientID);
        }

        private void RegisterClient(ulong clientID)
        {
            Managers.Logger.Log<ServerManager>($"Client(ID : {clientID}) regist", colorName: ColorCodes.Server);

            Draft.AddState(clientID);
        }
    }
}