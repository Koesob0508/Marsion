using Marsion.Server;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class ServerManager : MonoBehaviour
    {
        private Dictionary<ulong, ClientData> clientList = new();
        private int ReadyCount = 0;

        [SerializeField] GameServerEx _GameServerEx;
        public GameServer Game;
        public IGameServer GameEx => _GameServerEx;
        public DraftServer Draft;

        public void Init()
        {
            Managers.Logger.Log<ServerManager>("Server Manager initialized", colorName: ColorCodes.Server);

            Managers.Network.OnConnect += OnConnect;
            Managers.Network.OnClientJoin += OnClientConnected;
            Managers.Network.OnClientQuit += OnClientDisconnected;
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

            GameEx.Init();

            RegisterClient(Managers.Network.ClientID);
        }

        private void Clear()
        {
            Managers.Logger.Log<ServerManager>("Server Manager cleared", colorName: ColorCodes.Server);

            Game.Clear();
            Managers.Network.OnClientJoin -= OnClientConnected;
        }

        private void OnClientConnected(ulong clientID)
        {
            RegisterClient(clientID);
        }

        private void OnClientDisconnected(ulong clientID)
        {

        }

        private void RegisterClient(ulong clientID)
        {
            Managers.Logger.Log<ServerManager>($"Client(ID : {clientID}) regist", colorName: ColorCodes.Server);
            ClientData iclient = new ClientData(clientID);
            clientList[clientID] = iclient;

            Draft.AddState(clientID);
        }

        public void ReadyClient(ulong clientID, List<string> deck)
        {
            ClientData client = GetClient(clientID);
            client.State = ClientState.Ready;
            client.Deck = deck;

            GameEx.AddPlayer(client);
        }

        /// <summary>
        ///     원래라면, 해당 게임에 참여한 ClientID는 모두 End 처리를 해줘야 한다.
        /// </summary>
        public void EndGame()
        {
            ReadyCount = 0;

            foreach(var clientID in clientList.Keys)
            {
                var client = GetClient(clientID);
                // 원래는 DraftServer로부터 State를 받아서 끝내던가, Run을 지속하던가 해야한다.
                client.State = ClientState.Draft;
            }
        }

        public ClientData GetClient(ulong clientID)
        {
            if(clientList.TryGetValue(clientID, out var client))
                return client;
            else
                return null;
        }
    }

    public enum ClientState
    {
        Initial,
        Draft,
        Ready,
        Playing
    }

    public class ClientData
    {
        public ulong ClientID;
        public ClientState State = ClientState.Initial;
        public List<string> Deck;

        public ClientData(ulong id) { ClientID = id; }
    }
}