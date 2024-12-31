using Marsion.Logic;
using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Marsion
{
    public class DefaultGameLogic : IGameLogic
    {
        private IGameDataHandler _dataHandler;
        private readonly CommandInvoker _commandInvoker;

        public IDataManager Data { get; private set; }
        public IGameData GameData => _dataHandler.GameData;

        public event Action<IGameData> OnDataUpdated;
        public event Action OnGameStarted;
        public event Action OnManaChanged;
        public event Action OnTurnStarted;
        public event Action OnTurnEnded;
        public event Action<ulong, string> OnCardDrawn;
        public event Action<bool, ulong, string> OnCardPlayed;
        public event Action<bool, ulong, string, int> OnCardSpawned;
        public event Action<bool, ulong, string, ulong, string> OnCardAttacked;
        public event Action<List<string>> OnCardDied;
        public event Action<ulong> OnGameEnded;

        public DefaultGameLogic()
        {
            _commandInvoker = new();
        }

        public void Init(IGameLogicFactory logicFactory)
        {
            // Managers 역할
            Data = logicFactory.ProvideDataManager();

            _dataHandler = logicFactory.CreateGameDataHandler();
            var logicConfig = logicFactory.CreateGameLogicConfig();
            _dataHandler.Init(logicFactory.CreateGameDataHandlerFactory(this, logicConfig));
        }

        public void StartGame()
        {
            Logger.Log<DefaultGameLogic>($"Start Game", colorName: ColorCodes.Logic);

            // TODO : 멀리건 추가
            _dataHandler.DrawCard(_dataHandler.CurrentPlayer.PlayerID, out var _, 3);
            _dataHandler.DrawCard(_dataHandler.GetOpponentPlayer(_dataHandler.CurrentPlayer.PlayerID), out var _, 4);

            // Send Data Update
            OnDataUpdated?.Invoke(GameData);
            // Send Start Game
            OnGameStarted?.Invoke();

            StartTurn();
        }

        public void EndGame()
        {

        }

        public void StartTurn()
        {
            Logger.Log<DefaultGameLogic>($"Start Turn", colorName: ColorCodes.Logic);

            _dataHandler.AdvanceTurn();

            if (_dataHandler.CurrentPlayer.MaxMana < 10)
            {
                _dataHandler.CurrentPlayer.IncreaseMaxMana(1);
            }

            _dataHandler.CurrentPlayer.RestoreAllMana();

            _dataHandler.DrawCard(_dataHandler.CurrentPlayer.PlayerID, out var card);

            OnDataUpdated?.Invoke(GameData);
            OnManaChanged?.Invoke();
            OnTurnStarted?.Invoke();
            OnCardDrawn?.Invoke(_dataHandler.CurrentPlayer.PlayerID, card.UID);
        }

        public void EndTurn()
        {
            Logger.Log<DefaultGameLogic>($"End turn", colorName: ColorCodes.Logic);

            _dataHandler.ChangeCurrentPlayer();

            OnDataUpdated?.Invoke(GameData);
            OnTurnEnded?.Invoke();

            StartTurn();
        }

        public void TrySpawnCard(ulong playerID, string cardUID, int index)
        {
            Logger.Log<IGameLogic>("Try spawn card", colorName: ColorCodes.Logic);

            var player = _dataHandler.GetPlayer(playerID);
            var card = _dataHandler.GetCardFromHand(playerID, cardUID);

            if (!(player.Mana >= card.ManaCost))
            {
                Logger.Log<DefaultGameLogic>("Spawn try failed", colorName: ColorCodes.Logic);
                OnCardPlayed?.Invoke(false, player.PlayerID, card.UID);
                OnCardSpawned?.Invoke(false, player.PlayerID, card.UID, index);

                return;
            }

            var playCardCommand = new PlayCardCommand(player, card, index);
            _commandInvoker.AddCommand(playCardCommand);
            _commandInvoker.ExecuteCommands();

            OnCardPlayed?.Invoke(true, player.PlayerID, card.UID);
            OnCardSpawned?.Invoke(true, player.PlayerID, card.UID, index);
            OnDataUpdated?.Invoke(GameData);
            OnManaChanged?.Invoke();
        }

        public void TryAttack(ulong attackPlayerID, string attackCardUID, ulong defendPlayerID, string defendCardUID)
        {
            var attackPlayer = _dataHandler.GetPlayer(attackPlayerID);
            var attacker = _dataHandler.GetCardFromField(attackPlayerID, attackCardUID);
            var defendPlayer = _dataHandler.GetPlayer(defendPlayerID);
            var defender = _dataHandler.GetCardFromField(defendPlayerID, defendCardUID);

            var attackCommand = new AttackCommand(attackPlayer, attacker, defendPlayer, defender);
            _commandInvoker.AddCommand(attackCommand);
            _commandInvoker.ExecuteCommands();

            OnDataUpdated?.Invoke(GameData);
            OnCardAttacked?.Invoke(true, attackPlayer.PlayerID, attacker.UID, defendPlayer.PlayerID, defender.UID);

            List<string> deadCardUIDs = CheckDeadCard();

            OnDataUpdated?.Invoke(GameData);
            OnCardDied?.Invoke(deadCardUIDs);

            RemoveDeadCard();

            OnDataUpdated?.Invoke(GameData);

            List<ulong> alivePlayerIDs = new();

            foreach(var playerID in _dataHandler.GameData.Players.Keys)
            {
                if(_dataHandler.GetPlayer(playerID).Health > 0)
                {
                    alivePlayerIDs.Add(playerID);
                }
            }

            if(alivePlayerIDs.Count == 1)
            {
                OnGameEnded?.Invoke(alivePlayerIDs[0]);
            }
            else if(alivePlayerIDs.Count == 0)
            {
                OnGameEnded?.Invoke(1000);
            }
        }

        private List<string> CheckDeadCard()
        {
            List<string> result = new();

            foreach (var playerID in _dataHandler.GameData.Players.Keys)
            {
                foreach (Card card in _dataHandler.GetPlayer(playerID).Field)
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

            foreach (var playerID in _dataHandler.GameData.Players.Keys)
            {
                foreach (Card card in _dataHandler.GetPlayer(playerID).Field)
                {
                    if (card.IsDead)
                    {
                        deadCards.Add(card);
                    }
                }
            }

            foreach(var card in deadCards)
            {
                foreach(var playerID in _dataHandler.GameData.Players.Keys)
                {
                    if(_dataHandler.GetPlayer(playerID).Field.Contains(card))
                    {
                        _dataHandler.GetPlayer(playerID).Field.Remove(card);
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
                _dataHandler.DrawCard(playerID, out var drawnCard);
                OnDataUpdated?.Invoke(GameData);
                OnCardDrawn?.Invoke(playerID, drawnCard.UID);
            }
        }
    }
}