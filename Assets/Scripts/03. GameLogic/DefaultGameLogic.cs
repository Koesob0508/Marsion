using Marsion.Logic;
using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Marsion
{
    public class DefaultGameLogic : IGameLogicEx
    {
        private IGameDataHandler _dataHandler;
        private readonly CommandInvoker _commandInvoker;

        public IGameData GameData => _dataHandler.GameData;

        public event Action OnDataUpdated;
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
            _dataHandler = logicFactory.CreateGameDataHandler();
            var logicConfig = logicFactory.CreateGameLogicConfig();
            _dataHandler.Init(logicFactory.CreateGameDataHandlerFactory(logicConfig));
        }

        public void StartGame()
        {
            Logger.Log<DefaultGameLogic>($"Start Game", colorName: ColorCodes.Logic);

            // playerID로 접근하는식으로 바꿀 필요 있다.
            foreach (var player in _dataHandler.GameData.Players)
            {
                _dataHandler.ShuffleDeck(player);
                _dataHandler.DrawCard(player, out var mulliganTarget, 3);
            }

            // 후 플레이어는 카드 한 장 드로우
            _dataHandler.DrawCard(_dataHandler.GetOpponentPlayer(_dataHandler.CurrentPlayer.PlayerID), out var _);

            // Send Data Update
            OnDataUpdated?.Invoke();
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

            _dataHandler.DrawCard(_dataHandler.CurrentPlayer, out var card);

            OnDataUpdated?.Invoke();
            OnManaChanged?.Invoke();
            OnTurnStarted?.Invoke();
            OnCardDrawn?.Invoke(_dataHandler.CurrentPlayer.PlayerID, card.UID);
        }

        public void EndTurn()
        {
            Logger.Log<DefaultGameLogic>($"End turn", colorName: ColorCodes.Logic);

            _dataHandler.ChangeCurrentPlayer();

            OnDataUpdated?.Invoke();
            OnTurnEnded?.Invoke();

            StartTurn();
        }

        private void ShuffleDeck(Player player) => _dataHandler.ShuffleDeck(player);

        public void DrawCard(Player player, out Card drawnCard)
        {
            Logger.Log<GameLogic>("Draw a card", colorName: ColorCodes.Logic);

            _dataHandler.DrawCard(player, out drawnCard);
        }

        public void DrawCard(Player player, out List<Card> drawnCards, int count = 1)
        {
            Logger.Log<DefaultGameLogic>("Draw cards", colorName: ColorCodes.Logic);

            _dataHandler.DrawCard(player, out drawnCards, count);
        }

        public void TrySpawnCard(ulong playerID, string cardUID, int index)
        {
            Logger.Log<GameLogic>("Try spawn card", colorName: ColorCodes.Logic);

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
            OnDataUpdated?.Invoke();
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

            OnDataUpdated?.Invoke();
            OnCardAttacked?.Invoke(true, attackPlayer.PlayerID, attacker.UID, defendPlayer.PlayerID, defender.UID);

            List<string> deadCardUIDs = CheckDeadCard();

            OnDataUpdated?.Invoke();
            OnCardDied?.Invoke(deadCardUIDs);

            RemoveDeadCard();

            OnDataUpdated?.Invoke();

            List<ulong> alivePlayerIDs = new();

            foreach(var player in _dataHandler.GameData.Players)
            {
                if(player.Health > 0)
                {
                    alivePlayerIDs.Add(player.PlayerID);
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

            foreach (var player in _dataHandler.GameData.Players)
            {
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

            foreach (var player in _dataHandler.GameData.Players)
            {
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
                foreach(var player in _dataHandler.GameData.Players)
                {
                    if(player.Field.Contains(card))
                    {
                        player.Field.Remove(card);
                    }
                }
            }
        }

        public void Clear()
        {

        }
    }
}