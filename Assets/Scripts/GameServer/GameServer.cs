using Unity.Netcode;
using Marsion.Logic;
using UnityEngine;
using System;
using Marsion.Tool;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEditor.Timeline;

namespace Marsion.Server
{
    public class GameServer : NetworkBehaviour, IGameServer
    {
        [Header("Player")]
        [SerializeField] private DeckSO PlayerDeck;

        [Header("Enemy")]
        [SerializeField] private DeckSO EnemyDeck;

        private GameData GameData;
        private IGameLogic Logic;

        MyTween.MainSequence ServerSequence;

        // Event
        public event Action<NetworkGameData> OnDataUpdated;

        public event Action OnGameStarted;
        public event Action<int> OnGameEnded;
        public event Action OnTurnStarted;
        public event Action OnTurnEnded;

        public event Action<ulong, string> OnCardDrawn;
        public event Action OnManaChanged;
        public event Action<bool, ulong, string> OnCardPlayed;
        public event Action<bool, ulong, string, int> OnCardSpawned;
        public event Action<ulong, string, ulong, string> OnStartAttack;
        public event Action OnBeforeCardDead;
        public event Action OnAfterCardDead;

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

            ServerSequence = new MyTween.MainSequence();
        }

        public void Clear()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= ClientConnected;
            }
        }

        // Flow
        private void StartGame()
        {
            MyTween.Sequence sequence = new MyTween.Sequence();

            MyTween.Task startGameTask = new MyTween.Task(autoComplete: true);
            MyTween.Task drawCardTask = new MyTween.Task(autoComplete: true);
            MyTween.Task startTurnTask = new MyTween.Task(autoComplete: true);

            startGameTask.Action = () =>
            {
                Managers.Logger.Log<GameServer>("Start game", colorName: "blue");

                GameData = new GameData(2);
                Logic = new GameLogic();
                NetworkGameData networkData = new NetworkGameData();

                Logic.SetDeck(GetPlayer(0), PlayerDeck);
                Logic.SetDeck(GetPlayer(1), EnemyDeck);

                for(int i = 0; i < 2; i++)
                {
                    var player = GetPlayer((ulong)i);

                    Logic.SetPortrait(player, i);
                    Logic.ShuffleDeck(player);
                    Logic.SetHP(player, 30);
                }

                GameData.CurrentPlayer = GetPlayer(0);

                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnGameStarted?.Invoke();
            };

            drawCardTask.Action = () =>
            {
                Managers.Logger.Log<GameServer>("Draw initial card", colorName: "blue");

                NetworkGameData networkData = new NetworkGameData();

                for (int i = 0; i < 2; i++)
                {
                    var player = GetPlayer((ulong)i);

                    for (int j = 0; j < 3; j++)
                    {
                        Card card = Logic.DrawCard(player);

                        networkData.gameData = GameData;
                        OnDataUpdated?.Invoke(networkData);
                        OnCardDrawn?.Invoke(player.ClientID, card.UID);
                    }
                }

                Card exCard = Logic.DrawCard(GetPlayer(1));
                OnDataUpdated?.Invoke(networkData);
                OnCardDrawn?.Invoke(GetPlayer(1).ClientID, exCard.UID);
            };

            startTurnTask.Action = () =>
            {
                StartTurn();
            };

            sequence.Append(startGameTask);
            sequence.Append(drawCardTask);
            sequence.Append(startTurnTask);

            ServerSequence.Append(sequence);
            ServerSequence.Play();
        }

        private void EndGame()
        {
            Managers.Logger.Log<GameServer>("Game end", colorName: "blue");

            int winner = Logic.GetAlivePlayer();

            OnGameEnded?.Invoke(winner);
        }

        private void StartTurn()
        {
            MyTween.Sequence sequence = new MyTween.Sequence();
            MyTween.Task startTurnTask = new MyTween.Task(autoComplete: true);
            MyTween.Task drawCardTask = new MyTween.Task(autoComplete: true);
            NetworkGameData networkData = new NetworkGameData();

            startTurnTask.Action = () =>
            {
                Managers.Logger.Log<GameServer>("Start turn", colorName: "blue");

                GameData.TurnCount++;

                if (CurrentPlayer.MaxMana < 10)
                    CurrentPlayer.IncreaseMaxMana(1);

                CurrentPlayer.RestoreAllMana();

                networkData.gameData = GameData;

                OnTurnStarted?.Invoke();
                OnDataUpdated?.Invoke(networkData);
                OnManaChanged?.Invoke();
            };

            drawCardTask.Action = () =>
            {
                if (GameData.TurnCount != 1)
                {
                    Card card = Logic.DrawCard(CurrentPlayer);
                    networkData.gameData = GameData;
                    OnDataUpdated?.Invoke(networkData);
                    OnCardDrawn?.Invoke(CurrentPlayer.ClientID, card.UID);
                }
            };

            sequence.Append(startTurnTask);
            sequence.Append(drawCardTask);

            ServerSequence.Append(sequence);
            ServerSequence.Play();
        }

        private void EndTurn()
        {
            MyTween.Sequence sequence = new MyTween.Sequence();
            MyTween.Task endTurnTask = new MyTween.Task(autoComplete: true);
            MyTween.Task startTurnTask = new MyTween.Task(autoComplete: true);

            endTurnTask.Action = () =>
            {
                Managers.Logger.Log<GameLogic>("Turn end", colorName: "blue");

                GameData.CurrentPlayer = CurrentPlayer == GetPlayer(0) ? GetPlayer(1) : GetPlayer(0);

                NetworkGameData networkData = new NetworkGameData();
                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnTurnEnded?.Invoke();
            };

            startTurnTask.Action = () =>
            {
                StartTurn();
            };

            sequence.Append(endTurnTask);
            sequence.Append(startTurnTask);

            ServerSequence.Append(sequence);
            ServerSequence.Play();
        }

        [Rpc(SendTo.Server)]
        public void TurnEndRpc()
        {
            EndTurn();
        }

        [Rpc(SendTo.Server)]
        public void TryPlayAndSpawnCardRpc(ulong id, string cardUID, int index)
        {
            // Player의 턴이 아니라면 false
            // Field에 넣을 공간이 없다면 false
            // 현재 Player의 마나가 Card의 마나보다 적다면 false

            MyTween.Sequence sequence = new MyTween.Sequence();
            MyTween.Task task = new MyTween.Task(autoComplete: true);
            MyTween.Task updateTask = new MyTween.Task(autoComplete: true);
            MyTween.Task eventTask = new MyTween.Task(autoComplete: true);

            task.Action = () =>
            {
                var player = GetPlayer(id);
                var card = player.GetCard(cardUID);

                if (!(player.Mana >= card.Mana))
                {
                    Managers.Logger.Log<GameLogic>("그럴 수 없어요.", colorName: "blue");
                    OnCardPlayed?.Invoke(false, player.ClientID, card.UID);
                    OnCardSpawned?.Invoke(false, player.ClientID, card.UID, index);
                    return;
                }

                player.PayMana(card.Mana);
                player.Hand.Remove(card);
                player.Field.Insert(index, card);

                NetworkGameData networkData = new NetworkGameData();
                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnManaChanged?.Invoke();
                OnCardPlayed?.Invoke(true, player.ClientID, card.UID);
                OnCardSpawned?.Invoke(true, player.ClientID, card.UID, index);
            };

            sequence.Append(task);

            ServerSequence.Append(sequence);
            ServerSequence.Play();
        }

        [Rpc(SendTo.Server)]
        public void TryAttackRpc(ulong attackPlayer, string attackerUID, ulong defendPlayer, string defenderUID)
        {
            MyTween.Sequence sequence = new MyTween.Sequence();
            MyTween.Task task = new MyTween.Task();

            task.Action = () =>
            {
                Managers.Logger.Log<GameServer>("Try attack", colorName: "blue");

                var attP = GetPlayer(attackPlayer);
                var att = attP.GetCard(attackerUID);
                var defP = GetPlayer(defendPlayer);
                var def = defP.GetCard(defenderUID);

                Logic.Damage(att, def);
                NetworkGameData networkData = new NetworkGameData();
                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnStartAttack?.Invoke(attackPlayer, attackerUID, defendPlayer, defenderUID);

                bool result = Logic.CheckDeadCard(GameData.Players);

                networkData.gameData = GameData;
                OnDataUpdated?.Invoke(networkData);
                
                OnBeforeCardDead?.Invoke();
                OnAfterCardDead?.Invoke();

                //if (result)
                //    EndGame();
            };

            sequence.Append(task);

            ServerSequence.Append(sequence);
            ServerSequence.Play();
        }

        #region Utils

        private Player CurrentPlayer => GameData.CurrentPlayer;

        private bool AreAllPlayersConnected()
        {
            return Managers.Network.ConnectedClientsList.Count == 2;
        }

        private Player GetPlayer(ulong clientID)
        {
            return GameData.GetPlayer(clientID);
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

                StartGame();
            }
        }

        #endregion
    }
}