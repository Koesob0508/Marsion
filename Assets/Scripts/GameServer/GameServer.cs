using Unity.Netcode;
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

        private IServerGameData GameData;
        private IGameLogic Logic;

        // Action
        public event Action OnGameStarted;

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Logger.Log<GameServer>("Init server", colorName: "blue");

                Managers.Network.OnClientConnectedCallback -= ClientConnected;
                Managers.Network.OnClientConnectedCallback += ClientConnected;
            }
            else
            {
                Managers.Logger.Log<GameServer>("Network is null", colorName: "blue");
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost)
                Managers.Logger.Log<GameServer>("On network spawn", colorName: "blue");
        }

        private void ClientConnected(ulong clientId)
        {
            Managers.Logger.Log<GameServer>("Client connected", colorName: "blue");

            if (!IsHost)
            {
                Clear();
                gameObject.SetActive(false);
                return;
            }

            CheckConnectionRpc();
        }

        [Rpc(SendTo.Server)]
        private void CheckConnectionRpc()
        {
            Managers.Logger.Log<GameServer>($"Server : {Managers.Network.ConnectedClientsList.Count}", colorName: "blue");

            if (AreAllPlayersConnected())
            {
                Managers.Logger.Log<GameServer>($"Ready to start", colorName: "blue");

                //SetPlayerPortrait(GameData.Players[0], 0);
                //SetPlayerDeck(GameData.Players[0], Player_Deck);

                //SetPlayerPortrait(GameData.Players[1], 1);
                //SetPlayerDeck(GameData.Players[1], Enemy_Deck);

                //// 게임 시작
                //Logic.StartGame();

                StartGame();
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

        // Flow
        private void StartGame()
        {
            Managers.Logger.Log<GameServer>("Start game", colorName: "blue");

            OnGameStarted?.Invoke();
        }

        private void DataUpdated()
        {
            //GameData = Logic.GetGameData();
            //NetworkGameData networkData = new NetworkGameData();
            //networkData.gameData = GameData;

            //Managers.Client.UpdateDataRpc(networkData);
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
            //if (GameData == null) Managers.Logger.Log<GameServer>("Game data is null");
            //if (GameData.Players[clientID] == null) Managers.Logger.Log<GameServer>("Players is null");
            //return GameData.Players[clientID];

            return new Player((int)clientID);
        }

        #region Buttons
        [Rpc(SendTo.Server)]
        public void DrawButtonRpc(ulong clientID)
        {
            Logic.DrawCard(GetPlayer(clientID));
        }

        #endregion
    }
}