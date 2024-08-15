using Marsion.CardView;
using Marsion.Logic;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


namespace Marsion.Client
{
    public class ClientManager : NetworkBehaviour
    {
        #region Resource Fields

        public List<Sprite> PortraitSprites;

        #endregion
        private GameData gameData;
        public ulong ID;

        [SerializeField] HandView hand;
        [SerializeField] FieldView field;

        public IHandView Hand { get => hand; }
        public IFieldView Field { get => field; }

        public InputManager Input { get; private set; }

        public UnityAction OnGameStarted;
        public UnityAction OnDataUpdated;
        public UnityAction<ulong, string> OnCardDrawn;
        public UnityAction<ulong, string> OnCardPlayed;
        public UnityAction<ulong, string, int> OnCardSpawned;

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
            ID = Managers.Network.LocalClientId;
        }

        #region RPCs

        [Rpc(SendTo.ClientsAndHost)]
        public void GameStartRpc()
        {
            Managers.Logger.Log<ClientManager>("Game Start");

            OnGameStarted?.Invoke();
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

            OnDataUpdated?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DrawCardRpc(ulong clientID, string cardUID)
        {
            OnCardDrawn?.Invoke(clientID, cardUID);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayCardRpc(ulong clientID, string cardUID)
        {
            OnCardPlayed?.Invoke(clientID, cardUID);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnCardRpc(ulong clientID, string cardUID, int index)
        {
            OnCardSpawned?.Invoke(clientID, cardUID, index);
        }

        #endregion

        #region Utils

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

        public Card GetCardAtHand(ulong clientID, string cardUID)
        {
            foreach(Card card in GetGameData().Players[clientID].Hand)
            {
                if (card.UID == cardUID)
                {
                    Managers.Logger.Log<ClientManager>($"Get Card UID : {card.UID}");
                    return card;
                }
            }

            return null;
        }

        public Card GetCardAtField(ulong clientID, string cardUID)
        {
            foreach (Card card in GetGameData().Players[clientID].Field)
            {
                if (card.UID == cardUID)
                {
                    Managers.Logger.Log<ClientManager>($"Get Card UID : {card.UID}");
                    return card;
                }
            }

            return null;
        }

        #endregion

        public bool TryPlayCard(Card card)
        {
            return true;
        }

        public void PlayCard(Card card)
        {
            Managers.Server.PlayCardRpc(ID, card.UID);
        }

        public void PlayAndSpawnCard(Card card, int index)
        {
            Managers.Server.PlaySpawnCardRpc(ID, card.UID, index);
        }

        public void PlayCard(int index)
        {
            //Managers.Server.PlayCardRpc(ID, index);
            
            PlayCard(Managers.Client.GetPlayer().Hand[index]);
        }
    }
}