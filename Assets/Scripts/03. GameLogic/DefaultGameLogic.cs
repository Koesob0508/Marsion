using Marsion.Logic;
using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Marsion
{
    public class DefaultGameLogic : IGameLogic
    {
        public IDataManager Data { get; private set; }
        public ITriggerHandler Trigger { get; private set; }
        public IGameDataHandler DataHandler { get; private set; }
        public ICommandHandler CommandHandler { get; private set; }
        public ICommandFactory CommandFactory { get; private set; }

        public IGameData GameData => DataHandler.GameData;

        public event Action<IGameData> SendDataUpdated;
        public event Action SendGameStarted;
        public event Action SendTurnStarted;
        public event Action SendTurnEnded;
        public event Action<ulong, string> SendCardDrawn;
        public event Action<bool, ulong, string, int> SendCardSpawned;
        public event Action<bool, ulong, string, ulong, string> SendCardAttacked;
        public event Action<List<string>> SendCardDied;
        public event Action<ulong> SendGameEnded;

        public void Init(IGameLogicFactory logicFactory)
        {
            // Managers 역할
            Data = logicFactory.ProvideDataManager();
            Trigger = logicFactory.CreateTriggerhandler();
            DataHandler = logicFactory.CreateGameDataHandler();
            var logicConfig = logicFactory.CreateGameLogicConfig();
            CommandHandler = logicFactory.CreateCommandHandler(this);
            CommandFactory = logicFactory.CreateCommandFactory(this);

            DataHandler.Init(logicFactory.CreateGameDataHandlerFactory(this, logicConfig));
        }

        public void SubscribeEvent(Action<string, CommandData> observerAlert)
        {
            // TriggerHandler.SubscribeEvent(observerAlert);
        }

        public void UnsubscribeEvent(Action<string, CommandData> observerAlert)
        {
            // TriggerHandler.UnsubscribeEvent(observerAlert);
        }

        public void StartGame()
        {
            Logger.Log<DefaultGameLogic>($"Start Game", colorName: ColorCodes.Logic);

            // TODO : 멀리건 추가
            DataHandler.DrawCard(DataHandler.CurrentPlayer.PlayerID, out var _, count : 3);
            DataHandler.DrawCard(DataHandler.GetOpponentPlayerID(DataHandler.CurrentPlayer.PlayerID), out var _, count : 4);

            SendDataUpdated?.Invoke(GameData);
            SendGameStarted?.Invoke();

            StartTurn();
        }

        public void EndGame()
        {

        }

        public void StartTurn()
        {
            Logger.Log<DefaultGameLogic>($"Start Turn", colorName: ColorCodes.Logic);

            DataHandler.AdvanceTurn();

            if (DataHandler.CurrentPlayer.MaxMana < 10)
            {
                DataHandler.CurrentPlayer.IncreaseMaxMana(1);
            }

            DataHandler.CurrentPlayer.RestoreAllMana();

            DataHandler.DrawCard(DataHandler.CurrentPlayer.PlayerID, out var card);

            //TriggerHandler.TriggerEvent("UpdateData");
            //TriggerHandler.TriggerEvent("ChangeMana");
            //SendTurnStarted?.Invoke();
            //SendCardDrawn?.Invoke(DataHandler.CurrentPlayer.PlayerID, card.UID);
        }

        public void EndTurn()
        {
            //Logger.Log<DefaultGameLogic>($"End turn", colorName: ColorCodes.Logic);

            //DataHandler.ChangeCurrentPlayer();

            //SendDataUpdated?.Invoke(GameData);
            //SendTurnEnded?.Invoke();

            //StartTurn();
        }

        public void TrySpawnCard(ulong playerID, string cardUID, int index)
        {
            //Logger.Log<IGameLogic>("Try spawn card", colorName: ColorCodes.Logic);

            //var player = DataHandler.GetPlayer(playerID);
            //var card = DataHandler.GetCardFromHand(playerID, cardUID);

            //if (player.Mana < card.ManaCost)
            //{
            //    var cdata = new CommandData();
            //    cdata.PlayerID = player.PlayerID;
            //    cdata.CardUID = card.UID;

            //    //TriggerHandler.TriggerEvent("FailedPlayCard", cdata);
            //    return;
            //}

            //var payManaCommand = _commandFactory.CreatePayMana(playerID, card.ManaCost);
            //var spawnCommand = _commandFactory.CreateSpawnCard(playerID, cardUID, index);
            //_commandHandler.Add(payManaCommand);
            //_commandHandler.Add(spawnCommand);

            //SendCardSpawned?.Invoke(true, player.PlayerID, card.UID, index);
            //SendDataUpdated?.Invoke(GameData);
        }

        public void TryAttack(ulong attackPlayerID, string attackCardUID, ulong defendPlayerID, string defendCardUID)
        {
            //_commandHandler.Add(_commandFactory.CreateAttack(attackPlayerID, attackCardUID, defendPlayerID, defendCardUID));

            //SendDataUpdated?.Invoke(GameData);
            //SendCardAttacked?.Invoke(true, attackPlayerID, attackCardUID, defendPlayerID, defendCardUID);

            //List<string> deadCardUIDs = CheckDeadCard();

            //SendDataUpdated?.Invoke(GameData);
            //SendCardDied?.Invoke(deadCardUIDs);

            //RemoveDeadCard();

            //SendDataUpdated?.Invoke(GameData);

            //List<ulong> alivePlayerIDs = new();

            //foreach(var playerID in DataHandler.GameData.Players.Keys)
            //{
            //    if(DataHandler.GetPlayer(playerID).Health > 0)
            //    {
            //        alivePlayerIDs.Add(playerID);
            //    }
            //}

            //if(alivePlayerIDs.Count == 1)
            //{
            //    SendGameEnded?.Invoke(alivePlayerIDs[0]);
            //}
            //else if(alivePlayerIDs.Count == 0)
            //{
            //    SendGameEnded?.Invoke(1000);
            //}
        }

        private List<string> CheckDeadCard()
        {
            List<string> result = new();

            foreach (var playerID in DataHandler.GameData.Players.Keys)
            {
                if (!DataHandler.TryGetPlayer(playerID, out var player)) return null;

                foreach (Card card in player.Field)
                {
                    if(card.Health <= 0)
                    {
                        card.Die();
                        result.Add(card.UID);
                    }
                }
            }

            return result;
        }

        private void RemoveDeadCard()
        {
            List<Card> deadCards = new();

            foreach (var playerID in DataHandler.GameData.Players.Keys)
            {
                if (!DataHandler.TryGetPlayer(playerID, out var player)) return;

                foreach (Card card in player.Field)
                {
                    if (card.IsDead)
                    {
                        deadCards.Add(card);
                    }
                }
            }

            foreach(var card in deadCards)
            {
                foreach(var playerID in DataHandler.GameData.Players.Keys)
                {
                    if (!DataHandler.TryGetPlayer(playerID, out var player)) return;

                    if (player.Field.Contains(card))
                    {
                        player.Field.Remove(card);
                    }
                }
            }
        }

        public void Clear()
        {

        }

        public void DrawCard(ulong playerID, int count)
        {
            for (int i = 0; i < count; i++)
            {
                DataHandler.DrawCard(playerID, out var drawnCard);
                SendDataUpdated?.Invoke(GameData);
                SendCardDrawn?.Invoke(playerID, drawnCard.UID);
            }
        }
    }
}