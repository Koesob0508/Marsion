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
        public event UnityAction  OnGameEnded;

        public event UnityAction<Player, Card> OnCardDrawn;
        public event UnityAction <Player, Card> OnCardPlayed;
        public event UnityAction <Player, Card, int> OnCardSpawned;

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
                for(int i = 0; i < 8; i++)
                    DrawCard(player);
            }

            Managers.Logger.Log<GameLogic>("Game Start");
            UpdateData();
            OnGameStarted?.Invoke();
        }

        public void EndGame()
        {
            OnGameEnded?.Invoke();
        }

        public void StartTurn()
        {
            OnTurnStarted?.Invoke();
        }

        public void EndTurn()
        {
            OnTurnEnded?.Invoke();
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
            }

            UpdateData();
            OnCardDrawn?.Invoke(player, card);
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

        #endregion
    }
}