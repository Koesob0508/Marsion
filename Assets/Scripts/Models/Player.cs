using System.Collections.Generic;

namespace Marsion
{
    public class Player
    {
        public List<Card> Hand;
        public List<Card> Deck;
        public List<Card> Field;
        public List<Card> Grave;

        public Player()
        {
            Hand = new List<Card>();
            Deck = new List<Card>();
            Field = new List<Card>();
            Grave = new List<Card>();
        }
    }
}