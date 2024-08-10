using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion.Client
{
    public class ClientManager : NetworkBehaviour
    {
        private GameData gameData;
        public ulong ID;

        public InputManager Input { get; private set; }
        public DeckView Deck;

        public UnityAction OnStartGame;
        public UnityAction OnUpdateData;
        public UnityAction<ulong, int> OnDrawCard;

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
            Deck.Init();
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
            ID = Managers.Network.LocalClientId;
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void GameStartRpc()
        {
            Managers.Logger.Log<ClientManager>("Game Start");

            OnStartGame?.Invoke();
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void UpdateDataRpc(NetworkGameData networkData)
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

        [Rpc(SendTo.ClientsAndHost)]
        public void DrawCardRpc(ulong clientID, int count)
        {
            OnDrawCard?.Invoke(clientID, count);
        }

        private Player GetPlayer()
        {
            return gameData.Players[ID];
        }

        public GameData GetGameData()
        {
            return gameData;
        }

        public void GetHand()
        {
            var player = GetPlayer();
            Managers.Logger.Log<ClientManager>(player.LogPile(player.Hand));
        }

        public void GetField()
        {
            var player = GetPlayer();
            Managers.Logger.Log<ClientManager>(player.LogPile(player.Field));
        }

        public void PlayCard(int index)
        {
            Managers.Server.PlayCardRpc(ID, index);
        }
    }
}