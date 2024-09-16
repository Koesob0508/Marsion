﻿using Unity.Netcode;
using Marsion.Logic;
using UnityEngine;
using Marsion.Tool;

namespace Marsion.Server
{
    public class GameServer : NetworkBehaviour, IGameServer
    {
        public bool IsConnected { get; private set; }

        [Header("Sequencer")]
        [SerializeField] private Sequencer Sequencer;

        [Header("Player")]
        [SerializeField] private DeckSO PlayerDeck;

        [Header("Enemy")]
        [SerializeField] private DeckSO EnemyDeck;

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

                Logic.OnManaChanged -= ManaChanged;
                Logic.OnManaChanged += ManaChanged;

                Logic.OnCardPlayed -= CardPlayed;
                Logic.OnCardPlayed += CardPlayed;

                Logic.OnCardSpawned -= CardSpawned;
                Logic.OnCardSpawned += CardSpawned;

                Logic.OnStartAttack -= StartAttack;
                Logic.OnStartAttack += StartAttack;

                Logic.OnCardBeforeDead -= CardBeforeDead;
                Logic.OnCardBeforeDead += CardBeforeDead;

                Logic.OnCardAfterDead -= CardAfterDead;
                Logic.OnCardAfterDead += CardAfterDead;
            }
        }

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= ClientConnected;
                Managers.Network.OnClientConnectedCallback += ClientConnected;
            }
            else
            {
                Managers.Logger.Log<GameServer>("Network is null", colorName: "yellow");
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

            Managers.Client.UpdateDataRpc(networkData);
        }

        private void GameStarted()
        {
            Managers.Client.StartGameRpc();
        }

        private void GameEnded(int playerID)
        {
            Managers.Client.EndGameRpc(playerID);
        }

        private void TurnStarted()
        {
            Managers.Client.StartTurnRpc();
        }

        private void TurnEnded()
        {
            Managers.Client.EndTurnRpc();
        }

        private void CardDrawn(Player player, Card card)
        {
            Managers.Client.DrawCardRpc(player.ClientID, card.UID);
        }

        private void ManaChanged()
        {
            Managers.Client.ChangeManaRpc();
        }

        private void CardPlayed(bool succeeded, Player player, Card card)
        {
            Managers.Client.PlayCardRpc(succeeded, player.ClientID, card.UID);
        }

        private void CardSpawned(bool succeeded, Player player, Card card, int index)
        {
            Managers.Client.SpawnCardRpc(succeeded, player.ClientID, card.UID, index);
        }

        private void CardBeforeDead()
        {
            Managers.Client.BeforeDeadCardRpc();
        }

        private void CardAfterDead()
        {
            Managers.Client.AfterDeadCardRpc();
        }

        private void StartAttack(Card attacker, Card defender)
        {
            Managers.Client.StartAttackRpc(attacker.PlayerID, attacker.UID, defender.PlayerID, defender.UID);
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
                SetPlayerDeck(gameData.Players[0], PlayerDeck);

                SetPlayerPortrait(gameData.Players[1], 1);
                SetPlayerDeck(gameData.Players[1], EnemyDeck);

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
            Card card = player.GetCard(cardUID);
            Logic.PlayCard(player, card);
        }

        [Rpc(SendTo.Server)]
        public void TryPlayAndSpawnCardRpc(ulong clientID, string cardUID, int index)
        {
            Player player = GetPlayer(clientID);
            Card card = player.GetCard(cardUID);
            Logic.TryPlayAndSpawnCard(player, card, index);
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
            Card attacker = attackPlayer.GetCard(attackerUID);
            Player defendPlayer = GetPlayer(defenderID);
            Card defender = defendPlayer.GetCard(defenderUID);

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