using Marsion;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion
{
    public class GameLogic
    {
        private GameData gameData;

        #region UnityActions

        public UnityAction OnGameStart;
        public UnityAction OnUpdate;
        public UnityAction OnCardDrawn;
        public UnityAction OnCardPlayed;

        #endregion

        public GameLogic(GameData _gameData)
        {
            gameData = _gameData;
        }

        public GameData GetGameData()
        {
            return gameData;
        }

        private void UpdateData()
        {
            OnUpdate?.Invoke();
        }

        public void StartGame()
        {
            foreach (var player in gameData.Players)
            {
                ShuffleDeck(player.Deck);
                Managers.Logger.Log<GameLogic>("Draw");
                DrawCard(player, 5);
            }

            Managers.Logger.Log<GameLogic>("Game Start");
            UpdateData();
            OnGameStart?.Invoke();
        }

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

        public void DrawCard(Player player, int count = 1)
        {
            for(int i = 0; i < count; i++)
            {
                if(player.Deck.Count > 0 && player.Hand.Count < 10)
                {
                    Card card = player.Deck[0];
                    player.Deck.RemoveAt(0);
                    player.Hand.Add(card);
                }
            }

            UpdateData();
            OnCardDrawn?.Invoke();
        }

        public void PlayCard(ulong clientID, Card card)
        {
            gameData.Players[clientID].Hand.Remove(card);
            gameData.Players[clientID].Field.Add(card);

            UpdateData();
            OnCardPlayed?.Invoke();
        }
    }
}