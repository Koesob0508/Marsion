using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion.Server
{
    public class GameFlow
    {
        private Player currentPlayer;
        private Player player1;
        private Player player2;

        #region UnityActions

        public UnityAction onGameStart;

        #endregion

        public void SetFirstPlayerDeck(DeckSO deck)
        {
            player1 = new Player();
            SetPlayerDeck(player1, deck);
        }

        public void SetSecondPlayerDeck(DeckSO deck)
        {
            player2 = new Player();
            SetPlayerDeck(player2, deck);
        }

        private void SetPlayerDeck(Player player, DeckSO deck)
        {
            player.deck.Clear();

            foreach (CardSO cardSO in deck.cards)
            {
                if (cardSO != null)
                {
                    Card card = new Card(cardSO);
                    player.deck.Add(card);
                }
            }
        }

        public void SetCurrentPlayer()
        {
            currentPlayer = player1;
        }

        public void InitialDeckShuffle()
        {
            ShuffleDeck(player1.deck);
            ShuffleDeck(player2.deck);
        }

        public void StartGame()
        {
            onGameStart?.Invoke();

            // 두 명의 플레이어에 대해서,
            // 덱을 일단 셔플해줘야겠죠. 다만 지금 당장은 셔플은 생각하지 말자...
            Managers.Logger.Log<GameFlow>(player1.LogDeck());
            Managers.Logger.Log<GameFlow>(player2.LogDeck());
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
    }
}