using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion.Logic
{
    public class GameLogic : IGameLogic
    {
        #region Logic Operations

        #endregion

        private List<Player> AlivePlayers = new List<Player>();

        #region Interface Operations

        // Game Operations
        public void SetPortrait(Player player, int index)
        {
            player.Portrait = index;
        }

        public void SetHP(Player player, int amount)
        {
            player.Card.SetHP(amount);
        }

        public void SetDeck(Player player, List<Card> deck)
        {
            player.Deck.Clear();

            foreach (Card card in deck)
            {
                if (card != null)
                {
                    player.Deck.Add(card);
                }
            }
        }

        public void ShuffleDeck(Player player)
        {
            List<Card> deck = player.Deck;

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

        public Card DrawCard(Player player)
        {
            Managers.Logger.Log<GameLogic>("Card draw", colorName: "yellow");

            Card card = null;

            if (player.Deck.Count > 0 && player.Hand.Count < 10)
            {
                card = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(card);
                foreach(var ability in card.Abilities)
                {
                    ability.Register();
                }
            }
            else
            {
                Managers.Logger.Log<GameLogic>("Can't draw", colorName: "yellow");
            }

            return card;
        }

        public void Damage(IDamageable attacker, IDamageable defender)
        {
            attacker.Damage(defender.Attack);
            defender.Damage(attacker.Attack);
        }

        public bool CheckDeadCard(Player[] players)
        {
            foreach (Player player in players)
            {
                if (player.Card.Health <= 0)
                {
                    player.Card.Die();
                }

                foreach (Card card in player.Field)
                {
                    if (card.Health <= 0)
                    {
                        card.Die();
                    }
                }
            }

            AlivePlayers.Clear();

            foreach (Player player in players)
            {
                if (!player.Card.IsDead)
                    AlivePlayers.Add(player);
            }

            if (AlivePlayers.Count != 2)
            {
                Managers.Logger.Log<GameLogic>("End game", colorName: "yellow");
                return true;
            }

            return false;
        }

        public int GetAlivePlayer()
        {
            int winner = -1;

            foreach (var player in AlivePlayers)
            {
                winner = (int)player.ClientID;
            }

            return winner;
        }

        private void RemoveDeadCard()
        {
            //foreach (Player player in _gameData.Players)
            //{
            //    for (int i = player.Field.Count - 1; i >= 0; i--)
            //    {
            //        if (player.Field[i].IsDead)
            //        {
            //            player.Field.RemoveAt(i);
            //        }
            //    }
            //}
        }

        #endregion
    }
}