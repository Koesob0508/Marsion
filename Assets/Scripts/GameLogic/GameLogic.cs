using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion
{
    public class GameLogic
    {
        private Player currentPlayer;
        private Player player1;
        private Player player2;

        private GameData gameData;

        #region UnityActions

        public UnityAction onGameStart;
        public UnityAction onCardDrawn;

        #endregion

        public GameLogic(GameData _gameData)
        {
            gameData = _gameData;
        }

        public void SetBothPlayerDeck(DeckSO deck1, DeckSO deck2)
        {
            SetPlayerDeck(gameData.players[0], deck1);
            SetPlayerDeck(gameData.players[1], deck2);
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
            // 카드 뽑기

            DrawCard(player1, 5);
            DrawCard(player2, 5);

            Managers.Logger.Log<GameLogic>(player1.LogPile(player1.hand));
            Managers.Logger.Log<GameLogic>(player2.LogPile(player2.hand));

            // 확실히 블랙보드가 하나 있어서, 그 

            onGameStart?.Invoke();
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
                if(player.deck.Count > 0 && player.hand.Count < 10)
                {
                    Card card = player.deck[0];
                    player.deck.RemoveAt(0);
                    player.hand.Add(card);
                }
            }

            onCardDrawn?.Invoke();
        }
    }
}