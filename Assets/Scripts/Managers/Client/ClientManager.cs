using Marsion.Client;
using Marsion;
using Marsion.Clinet;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion.Client
{
    public class ClientManager : NetworkBehaviour
    {
        private GameData gameData;
        private ulong clientID;

        public InputManager Input { get; private set; }

        public UnityAction OnStartGame;
        public UnityAction OnUpdateData;

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                Debug.Log($"This object is owned by client with ID: {NetworkObject.OwnerClientId}");
            }
        }

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= SetClientID;
                Managers.Network.OnClientConnectedCallback += SetClientID;
            }

            Input = new InputManager();
        }

        public void Update()
        {
            Input.Update();
        }

        public void Clear()
        {
            Managers.Network.OnClientConnectedCallback -= SetClientID;
        }

        public void SetClientID(ulong clientID)
        {
            this.clientID = Managers.Network.LocalClientId;
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void GameStartRpc()
        {
            Managers.Logger.Log<ClientManager>("Game Start");

            OnStartGame?.Invoke();
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void UpdateDataClientRpc(NetworkGameData networkData)
        {
            Managers.Logger.Log<ClientManager>("On update game data.");
            gameData = networkData.gameData;

            foreach (Player player in gameData.Players)
            {
                Managers.Logger.Log<ClientManager>(player.LogPile(player.Field));
                Managers.Logger.Log<ClientManager>(player.LogPile(player.Hand));
            }

            OnUpdateData?.Invoke();
        }

        public GameData GetGameData()
        {
            return gameData;
        }

        public void GetHand()
        {
            Managers.Logger.Log<ClientManager>(gameData.Players[clientID].LogPile(gameData.Players[clientID].Hand));
        }

        public void GetField()
        {
            Managers.Logger.Log<ClientManager>(gameData.Players[clientID].LogPile(gameData.Players[clientID].Field));
        }

        public void PlayCard(int index)
        {
            Managers.Server.PlayCardServerRpc(clientID, index);
        }
    }
}