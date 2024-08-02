using NUnit.Framework.Interfaces;
using System.Collections.Generic;

namespace Marsion
{
    public class Player
    {
        public List<Card> deck = new List<Card>();

        public string LogDeck()
        {
            string result = "";

            foreach(Card card in deck)
            {
                result += $"{card.Rank} {card.Suit}";
                result += "/";
            }

            return result;
        }
    }
}