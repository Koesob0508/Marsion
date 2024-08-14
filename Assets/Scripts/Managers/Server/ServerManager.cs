using Unity.Netcode;
using Marsion.Logic;
using UnityEngine;

namespace Marsion.Server
{
    public class ServerManager : NetworkBehaviour
    {
        public bool IsConnected { get; private set; }

        [Header("Player")]
        [SerializeField] private DeckSO Player_Deck;

        [Header("Enemy")]
        [SerializeField] private DeckSO Enemy_Deck;

        private GameData gameData;
        private GameLogic Logic;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(IsHost)
            {
                Managers.Logger.Log<ServerManager>("Init Server");

                int playerCount = 2;
                gameData = new GameData(playerCount);
                Logic = new GameLogic(gameData);

                Logic.OnGameStarted -= OnGameStart;
                Logic.OnGameStarted += OnGameStart;

                Logic.OnUpdated -= OnUpdateData;
                Logic.OnUpdated += OnUpdateData;

                Logic.OnCardDrawn -= OnCardDraw;
                Logic.OnCardDrawn += OnCardDraw;

                Logic.OnCardPlayed -= OnCardPlay;
                Logic.OnCardPlayed += OnCardPlay;

                Logic.OnCardSpawned -= OnCardSpawned;
                Logic.OnCardSpawned += OnCardSpawned;
            }
        }

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= OnClientConnected;
                Managers.Network.OnClientConnectedCallback += OnClientConnected;
            }
        }

        public void Clear()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= OnClientConnected;
            }
        }

        [ServerRpc]
        public void ClearServerRpc()
        {
            Logic.OnGameStarted -= OnGameStart;
            Logic.OnUpdated -= OnUpdateData;
            Logic.OnCardDrawn -= OnCardDraw;
        }

        /// <summary>
        ///     Server에서 발생하는 이벤트 처리
        /// </summary>
        /// <param name="clientId"></param>
        #region Server Flow

        private void OnClientConnected(ulong clientId)
        {
            if (!IsHost)
            {
                Clear();
                gameObject.SetActive(false);
                return;
            }

            CheckConnectionRpc();
        }

        #endregion

        /// <summary>
        ///     Logic에서 발생한 이벤트를 Client로 전달해주는 역할하는 메서드들 (중복 구현)
        /// </summary>
        #region Logic Flow

        private void OnGameStart()
        {
            Managers.Client.GameStartRpc();
        }

        private void OnUpdateData()
        {
            gameData = Logic.GetGameData();
            NetworkGameData networkData = new NetworkGameData();
            networkData.gameData = gameData;
            Managers.Client.UpdateDataRpc(networkData);
        }

        private void OnCardDraw(Player player, Card card)
        {
            Managers.Client.DrawCardRpc(player.ClientID, card.UID);
        }

        private void OnCardPlay(Player player, Card card)
        {
            Managers.Client.PlayCardRpc(player.ClientID, card.UID);
        }

        private void OnCardSpawned(Player player, Card card, int index)
        {
            Managers.Client.SpawnCardRpc(player.ClientID, card.UID, index);
        }

        #endregion

        /// <summary>
        ///     Server Manager가 처리해야할 작업
        /// </summary>
        #region Operations

        [Rpc(SendTo.Server)]
        private void CheckConnectionRpc()
        {
            if (AreAllPlayersConnected())
            {
                Managers.Logger.Log<ServerManager>($"Ready to start");

                SetPlayerPortrait(gameData.Players[0], 0);
                SetPlayerDeck(gameData.Players[0], Player_Deck);

                SetPlayerPortrait(gameData.Players[1], 1);
                SetPlayerDeck(gameData.Players[1], Enemy_Deck);

                // 게임 시작
                Logic.StartGame();
            }
        }

        [Rpc(SendTo.Server)]
        public void DrawCardRpc(ulong clientID)
        {
            Player player = GetPlayer(clientID);
            Logic.DrawCard(player);
        }

        [Rpc(SendTo.Server)]
        public void PlayCardRpc(ulong clientID, string cardUID)
        {
            Player player = GetPlayer(clientID);
            Card card = player.GetCard(player.Hand, cardUID);
            Logic.PlayCard(player, card);
        }

        [Rpc(SendTo.Server)]
        public void PlaySpawnCardRpc(ulong clientID, string cardUID, int index)
        {
            Player player = GetPlayer(clientID);
            Card card = player.GetCard(player.Hand, cardUID);
            Logic.PlayAndSpawnCard(player, card, index);
        }

        private bool AreAllPlayersConnected()
        {
            return Managers.Network.ConnectedClientsList.Count == 2;
        }

        private void SetPlayerPortrait(Player player, int spriteIndex)
        {
            player.Portrait = spriteIndex;
        }

        private void SetPlayerDeck(Player player, DeckSO deck)
        {
            player.Deck.Clear();

            foreach (CardSO cardSO in deck.cards)
            {
                if (cardSO != null)
                {
                    Card card = new Card(player.ClientID, cardSO);
                    player.Deck.Add(card);
                }
            }
        }

        private Player GetPlayer(ulong clientID)
        {
            return gameData.Players[clientID];
        }

        #endregion

        #region Buttons

        [Rpc(SendTo.Server)]
        public void DrawButtonRpc(ulong clientID)
        {
            Logic.DrawCard(GetPlayer(clientID));
        }

        #endregion
    }
}