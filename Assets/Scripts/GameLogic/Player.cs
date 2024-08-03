using NUnit.Framework.Interfaces;
using System.Collections.Generic;

namespace Marsion
{
    public class Player
    {
        public List<Card> deck = new List<Card>();
        public List<Card> hand = new List<Card>();

        public string LogPile(List<Card> pile)
        {
            string result = "";

            foreach(Card card in pile)
            {
                result += $"{card.Rank} {card.Suit}";
                result += "/";
            }

            return result;
        }
    }
}