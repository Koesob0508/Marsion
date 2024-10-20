using Unity.Netcode;
using Marsion.Logic;
using UnityEngine;
using System;
using Marsion.Tool;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Marsion.Server
{
    public class GameServer : NetworkBehaviour
    {
        private GameData GameData;

        public int ReadyPlayerCount;

        [Header("Sequencer")]
        [SerializeField] private Sequencer Sequencer;

        private ulong FirstPlayerClientID = 0;
        private ulong SecondPlayerClientID = 1;
        private List<Card> HostDeck;
        private List<Card> GuestDeck;

        private IGameLogic Logic;

        // Event
        public event Action<SerializedGameData> OnDataUpdated;

        public event Action OnGameStarted;
        public event Action<int> OnGameEnded;
        public event Action OnResetGame;
        public event Action OnTurnStarted;
        public event Action OnTurnEnded;

        public event Action<ulong, string> OnCardDrawn;
        public event Action OnManaChanged;
        public event Action<bool, ulong, string> OnCardPlayed;
        public event Action<bool, ulong, string, int> OnCardSpawned;
        public event Action<ulong, string, ulong, string> OnStartAttack;
        public event Action OnDeadCard;

        public void Init()
        {
            Managers.Logger.Log<GameServer>($"Game Server initialized", colorName: ColorCodes.Server);

            Sequencer.Init();
        }

        private void ResetGame()
        {
            ReadyPlayerCount = 0;
            Sequencer.Init();
            HostDeck.Clear();
            GuestDeck.Clear();
            GameData = null;
            Logic = null;
        }

        public void Clear()
        {

        }

        // Flow
        private void StartGame()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("StartGame", Sequencer);

            Sequencer.Clip startGameClip = new Sequencer.Clip("StartGame");
            Sequencer.Clip drawCardClip = new Sequencer.Clip("DrawCard");
            Sequencer.Clip startTurnClip = new Sequencer.Clip("StartTurn");

            startGameClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameServer>("Start game", colorName: ColorCodes.Server);

                GameData = new GameData(2);
                Logic = new GameLogic();
                SerializedGameData networkData = new SerializedGameData();

                Logic.SetDeck(GetPlayer(0), HostDeck);
                Logic.SetDeck(GetPlayer(1), GuestDeck);

                for(int i = 0; i < 2; i++)
                {
                    var player = GetPlayer((ulong)i);

                    Logic.SetPortrait(player, i);
                    Logic.ShuffleDeck(player);
                    Logic.SetHP(player, 30);
                }

                GameData.CurrentPlayer = GetPlayer(FirstPlayerClientID);

                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnGameStarted?.Invoke();
            };

            drawCardClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameServer>("Draw initial card", colorName: "#FFA500");

                SerializedGameData networkData = new SerializedGameData();

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

                Card exCard = Logic.DrawCard(GetPlayer(SecondPlayerClientID));
                OnDataUpdated?.Invoke(networkData);
                OnCardDrawn?.Invoke(GetPlayer(SecondPlayerClientID).ClientID, exCard.UID);
            };

            startTurnClip.OnPlay += () =>
            {
                StartTurn();
            };

            sequence.Append(startGameClip);
            sequence.Append(drawCardClip);
            sequence.Append(startTurnClip);

            Sequencer.Append(sequence);
        }

        private void EndGame()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("EndGame", Sequencer);
            Sequencer.Clip endGameClip = new Sequencer.Clip("EndGame");
            Sequencer.Clip resetServerClip = new Sequencer.Clip("ResetServer");

            endGameClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameServer>("Game end", colorName: "#FFA500");

                int winner = Logic.GetAlivePlayer();

                OnGameEnded?.Invoke(winner);
            };

            resetServerClip.OnPlay += () =>
            {
                ResetGame();

                OnResetGame?.Invoke();
            };

            sequence.Append(endGameClip);
            sequence.Append(resetServerClip);

            Sequencer.Append(sequence);
        }

        private void StartTurn()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("StartTurn", Sequencer);
            Sequencer.Clip startTurnClip = new Sequencer.Clip("StartTurn");
            Sequencer.Clip drawCardClip = new Sequencer.Clip("DrawCard");
            SerializedGameData networkData = new SerializedGameData();

            startTurnClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameServer>("Start turn", colorName: "#FFA500");

                GameData.TurnCount++;

                if (CurrentPlayer.MaxMana < 10)
                    CurrentPlayer.IncreaseMaxMana(1);

                CurrentPlayer.RestoreAllMana();

                networkData.gameData = GameData;

                OnTurnStarted?.Invoke();
                OnDataUpdated?.Invoke(networkData);
                OnManaChanged?.Invoke();
            };

            drawCardClip.OnPlay += () =>
            {
                Card card = Logic.DrawCard(CurrentPlayer);
                networkData.gameData = GameData;
                OnDataUpdated?.Invoke(networkData);
                OnCardDrawn?.Invoke(CurrentPlayer.ClientID, card.UID);
            };

            sequence.Append(startTurnClip);
            sequence.Append(drawCardClip);

            Sequencer.Append(sequence);
        }

        private void EndTurn()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("EndTurn", Sequencer);
            Sequencer.Clip endTurnClip = new Sequencer.Clip("EndTurn");
            Sequencer.Clip startTurnClip = new Sequencer.Clip("StartTurn");

            endTurnClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameLogic>("Turn end", colorName: "#FFA500");

                GameData.CurrentPlayer = CurrentPlayer == GetPlayer(0) ? GetPlayer(1) : GetPlayer(0);

                SerializedGameData networkData = new SerializedGameData();
                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnTurnEnded?.Invoke();
            };

            startTurnClip.OnPlay += () =>
            {
                StartTurn();
            };

            sequence.Append(endTurnClip);
            sequence.Append(startTurnClip);

            Sequencer.Append(sequence);
        }

        [Rpc(SendTo.Server)]
        public void ReadyRpc(ulong id, StringContainer[] deck)
        {
            Managers.Logger.Log<GameServer>($"Client({id}) ready", colorName: ColorCodes.Server);
            List<Card> resultDeck = new List<Card>();

            foreach(var soID in deck)
            {
                if(Managers.Data.CardDictionary.TryGetValue(soID.SomeText, out var cardSO))
                {
                    resultDeck.Add(new Card(id, cardSO));
                }
                else
                {
                    Managers.Logger.LogWarning<GameServer>($"ID : {soID} CardSO not found", colorName: ColorCodes.Server);
                }
            }

            switch(id)
            {
                case 0:
                    HostDeck = resultDeck;
                    break;
                case 1:
                    GuestDeck = resultDeck;
                    break;
            }

            switch (ReadyPlayerCount)
            {
                case 0:
                    FirstPlayerClientID = id;
                    ReadyPlayerCount++;
                    break;
                case 1:
                    SecondPlayerClientID = id;
                    StartGame();
                    break;
            }
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

            Sequencer.Sequence sequence = new Sequencer.Sequence("TryPlayAndSpawn", Sequencer);
            Sequencer.Clip clip = new Sequencer.Clip("TryPlayAndSpawn");
            Sequencer.Clip updateClip = new Sequencer.Clip("Update");
            Sequencer.Clip eventClip = new Sequencer.Clip("InvokeEvents");

            clip.OnPlay += () =>
            {
                var player = GetPlayer(id);
                var card = player.GetCard(cardUID);

                if (!(player.Mana >= card.Mana))
                {
                    Managers.Logger.Log<GameServer>("그럴 수 없어요.", colorName: "#FFA500");
                    OnCardPlayed?.Invoke(false, player.ClientID, card.UID);
                    OnCardSpawned?.Invoke(false, player.ClientID, card.UID, index);
                    return;
                }

                player.PayMana(card.Mana);
                player.Hand.Remove(card);
                player.Field.Insert(index, card);

                card.OnPlay?.Invoke();

                SerializedGameData networkData = new SerializedGameData();
                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnManaChanged?.Invoke();
                OnCardPlayed?.Invoke(true, player.ClientID, card.UID);
                OnCardSpawned?.Invoke(true, player.ClientID, card.UID, index);
            };

            sequence.Append(clip);

            Sequencer.Append(sequence);
        }

        [Rpc(SendTo.Server)]
        public void TryAttackRpc(ulong attackPlayer, string attackerUID, ulong defendPlayer, string defenderUID)
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("TryAttack", Sequencer);
            Sequencer.Clip tryAttackClip = new Sequencer.Clip("TryAttack");
            Sequencer.Clip checkDeadClip = new Sequencer.Clip("CheckDead");

            tryAttackClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameServer>("Try attack", colorName: "#FFA500");

                var attP = GetPlayer(attackPlayer);
                var att = attP.GetCard(attackerUID);
                var defP = GetPlayer(defendPlayer);
                var def = defP.GetCard(defenderUID);

                Logic.Damage(att, def);
                SerializedGameData networkData = new SerializedGameData();
                networkData.gameData = GameData;

                OnDataUpdated?.Invoke(networkData);
                OnStartAttack?.Invoke(attackPlayer, attackerUID, defendPlayer, defenderUID);
            };

            checkDeadClip.OnPlay += () =>
            {
                bool result = Logic.CheckDeadCard(GameData.Players);

                SerializedGameData networkData = new SerializedGameData();
                networkData.gameData = GameData;
                OnDataUpdated?.Invoke(networkData);

                OnDeadCard?.Invoke();

                if (result)
                    EndGame();
            };

            sequence.Append(tryAttackClip);
            sequence.Append(checkDeadClip);

            Sequencer.Append(sequence);
        }

        #region Utils

        private Player CurrentPlayer => GameData.CurrentPlayer;

        private bool AreAllPlayersConnected()
        {
            return Managers.Network.ConnectedClientsCount == 2;
        }

        private Player GetPlayer(ulong clientID)
        {
            return GameData.GetPlayer(clientID);
        }

        //[Rpc(SendTo.Server)]
        //public void CheckConnectionRpc()
        //{
        //    Managers.Logger.Log<GameServer>($"Server : {Managers.Network.ConnectedClientsCount}", colorName: "#FFA500");

        //    if (AreAllPlayersConnected())
        //    {
        //        Managers.Logger.Log<GameServer>($"Ready to start", colorName: "#FFA500");

        //        ReadyPlayerCount = 0;
        //    }
        //}

        #endregion
    }
}