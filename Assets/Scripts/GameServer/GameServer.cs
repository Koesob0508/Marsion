using Unity.Netcode;
using Marsion.Logic;
using UnityEngine;

namespace Marsion.Server
{
    public class GameServer : NetworkBehaviour, IGameServer
    {
        public bool IsConnected { get; private set; }

        [Header("Player")]
        [SerializeField] private DeckSO Player_Deck;

        [Header("Enemy")]
        [SerializeField] private DeckSO Enemy_Deck;

        private GameData gameData;
        private IGameLogic Logic;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(IsHost)
            {
                Managers.Logger.Log<GameServer>("Init Server");

                int playerCount = 2;
                gameData = new GameData(playerCount);
                Logic = new GameLogic(gameData);

                Logic.OnDataUpdated -= DataUpdated;
                Logic.OnDataUpdated += DataUpdated;
                
                Logic.OnGameStarted -= GameStarted;
                Logic.OnGameStarted += GameStarted;

                Logic.OnGameEnded -= GameEnded;
                Logic.OnGameEnded += GameEnded;

                Logic.OnTurnStarted -= TurnStarted;
                Logic.OnTurnStarted += TurnStarted;

                Logic.OnTurnEnded -= TurnEnded;
                Logic.OnTurnEnded += TurnEnded;

                Logic.OnCardDrawn -= CardDrawn;
                Logic.OnCardDrawn += CardDrawn;

                Logic.OnCardPlayed -= CardPlayed;
                Logic.OnCardPlayed += CardPlayed;

                Logic.OnCardSpawned -= CardSpawned;
                Logic.OnCardSpawned += CardSpawned;

                Logic.OnStartAttack -= StartAttack;
                Logic.OnStartAttack += StartAttack;

                Logic.OnCardDead -= CardDead;
                Logic.OnCardDead += CardDead;
            }
        }

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= ClientConnected;
                Managers.Network.OnClientConnectedCallback += ClientConnected;
            }
        }

        public void Clear()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= ClientConnected;
            }
        }

        [ServerRpc]
        public void ClearServerRpc()
        {
            Logic.OnGameStarted -= GameStarted;
            Logic.OnDataUpdated -= DataUpdated;
            Logic.OnCardDrawn -= CardDrawn;
        }

        /// <summary>
        ///     Server에서 발생하는 이벤트 처리
        /// </summary>
        /// <param name="clientId"></param>
        #region Server Flow

        private void ClientConnected(ulong clientId)
        {
            Managers.Logger.Log<GameServer>("Client connected");

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
        #region Event Rpcs
        private void DataUpdated()
        {
            gameData = Logic.GetGameData();
            NetworkGameData networkData = new NetworkGameData();
            networkData.gameData = gameData;

            foreach (Player player in gameData.Players)
            {
                foreach (Card card in player.Field)
                {
                    if (card.HP <= 0)
                        card.Die();

                    Managers.Logger.Log<GameServer>($"{gameData.GetFieldCard(player.ClientID, card.UID).IsDead}", colorName: "yellow");
                }
            }

            Managers.Client.UpdateDataRpc(networkData);
        }

        private void GameStarted()
        {
            Managers.Client.StartGameRpc();
        }

        private void GameEnded()
        {

        }

        private void TurnStarted()
        {

        }

        private void TurnEnded()
        {
            Managers.Client.EndTurnRpc();
        }

        private void CardDrawn(Player player, Card card)
        {
            Managers.Client.DrawCardRpc(player.ClientID, card.UID);
        }

        private void CardPlayed(Player player, Card card)
        {
            Managers.Client.PlayCardRpc(player.ClientID, card.UID);
        }

        private void CardSpawned(Player player, Card card, int index)
        {
            Managers.Client.SpawnCardRpc(player.ClientID, card.UID, index);
        }

        private void CardDead()
        {
            Managers.Client.DeadCardRpc();
        }


        private void StartAttack(Card attacker, Card defender)
        {
            Managers.Client.StartAttackRpc(attacker.ClientID, attacker.UID, defender.ClientID, defender.UID);
        }

        #endregion

        /// <summary>
        ///     Server Manager가 처리해야할 작업
        /// </summary>
        #region Operations

        [Rpc(SendTo.Server)]
        private void CheckConnectionRpc()
        {
            Managers.Logger.Log<GameServer>($"Server : {Managers.Network.ConnectedClientsList.Count}");

            if (AreAllPlayersConnected())
            {
                Managers.Logger.Log<GameServer>($"Ready to start");

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
        public void PlayAndSpawnCardRpc(ulong clientID, string cardUID, int index)
        {
            Player player = GetPlayer(clientID);
            Card card = player.GetCard(player.Hand, cardUID);
            Logic.PlayAndSpawnCard(player, card, index);
        }

        [Rpc(SendTo.Server)]
        public void TurnEndRpc()
        {
            Logic.EndTurn();
        }

        [Rpc(SendTo.Server)]
        public void TryAttackRpc(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID)
        {
            Player attackPlayer = GetPlayer(attackerID);
            Card attacker = attackPlayer.GetCard(attackPlayer.Field, attackerUID);
            Player defendPlayer = GetPlayer(defenderID);
            Card defender = defendPlayer.GetCard(defendPlayer.Field, defenderUID);

            Logic.TryAttack(attackPlayer, attacker, defendPlayer, defender);
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
            if (gameData == null) Managers.Logger.Log<GameServer>("Game data is null");
            if (gameData.Players[clientID] == null) Managers.Logger.Log<GameServer>("Players is null");
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