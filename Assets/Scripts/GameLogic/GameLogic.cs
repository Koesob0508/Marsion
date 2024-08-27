using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion.Logic
{
    public class GameLogic : IGameLogic
    {
        GameData _gameData;

        #region Logic Operations

        public GameLogic(GameData gameData)
        {
            _gameData = gameData;
        }

        private void UpdateData()
        {
            OnDataUpdated?.Invoke();
        }

        #endregion

        #region Interface Operations

        public event UnityAction OnDataUpdated;

        public event UnityAction OnGameStarted;
        public event UnityAction OnTurnStarted;
        public event UnityAction OnTurnEnded;
        public event UnityAction OnGameEnded;

        public event UnityAction<Player, Card> OnCardDrawn;
        public event UnityAction<Player, Card> OnCardPlayed;
        public event UnityAction<Player, Card, int> OnCardSpawned;
        public event UnityAction<Card, Card> OnStartAttack;
        public event UnityAction OnCardDead;

        public GameData GetGameData()
        {
            return _gameData;
        }

        // Flow
        public void StartGame()
        {
            foreach (var player in _gameData.Players)
            {
                ShuffleDeck(player.Deck);
                Managers.Logger.Log<GameLogic>("Draw");
                for(int i = 0; i < 3; i++)
                    DrawCard(player);
            }

            _gameData.CurrentPlayer = _gameData.GetPlayer(0);

            Managers.Logger.Log<GameLogic>("Game Start");
            UpdateData();
            OnGameStarted?.Invoke();

            StartTurn();
        }

        public void EndGame()
        {
            OnGameEnded?.Invoke();
        }

        public void StartTurn()
        {
            if(_gameData.TurnCount != 0)
                DrawCard(GetGameData().CurrentPlayer);

            _gameData.TurnCount++;

            UpdateData();
            OnTurnStarted?.Invoke();
        }

        public void EndTurn()
        {
            _gameData.CurrentPlayer = _gameData.CurrentPlayer == _gameData.GetPlayer(0) ? _gameData.GetPlayer(1) : _gameData.GetPlayer(0);

            UpdateData();
            OnTurnEnded?.Invoke();

            StartTurn();
        }

        // Game Operations

        public void ShuffleDeck(List<Card> deck)
        {
            Random rng = new Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        public void DrawCard(Player player)
        {
            Card card = null;

            if (player.Deck.Count > 0 && player.Hand.Count < 10)
            {
                card = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(card);

                UpdateData();
                OnCardDrawn?.Invoke(player, card);
            }
            else
            {
                Managers.Logger.Log<GameLogic>("Can't draw");
            }
        }

        public void PlayCard(Player player, Card card)
        {
            player.Hand.Remove(card);

            UpdateData();
            OnCardPlayed?.Invoke(player, card);
        }

        public void SpanwCard(Player player, Card card, int index)
        {
            player.Field.Insert(index, card);

            UpdateData();
            OnCardSpawned?.Invoke(player, card, index);
        }

        public void PlayAndSpawnCard(Player player, Card card, int index)
        {
            player.Hand.Remove(card);
            player.Field.Insert(index, card);

            UpdateData();
            OnCardPlayed?.Invoke(player, card);
            OnCardSpawned?.Invoke(player, card, index);
        }

        public void TryAttack(Player attackPlayer, Card attacker, Player defenderPlayer, Card defender)
        {
            // 각 Player Field 확인해서 조건 검색

            // OnBeforeAttack.Invoke(attacker, defender);
            Damage(attacker, defender);
            OnDataUpdated?.Invoke();
            OnStartAttack?.Invoke(attacker, defender);
            CheckDeadCard();
            OnDataUpdated?.Invoke();
            OnCardDead?.Invoke();
        }

        private void Damage(IDamageable attacker, IDamageable defender)
        {
            attacker.Damage(defender.Attack);
            defender.Damage(attacker.Attack);
        }

        private void CheckDeadCard()
        {
            foreach(Player player in _gameData.Players)
            {
                foreach(Card card in player.Field)
                {
                    if (card.HP <= 0)
                        card.Die();
                }
            }
        }

        #endregion
    }
}