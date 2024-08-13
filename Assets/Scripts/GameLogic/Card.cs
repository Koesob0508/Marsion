
using System;

namespace Marsion.Logic
{
    [Serializable]
    public enum Suit
    {
        Spade,
        Heart,
        Diamond,
        Club
    }

    [Serializable]
    public class Card
    {
        //public int ID;
        //public string Name;
        //public int Mana;
        //public int Marsion;
        //public int Attack;
        //public int Health;

        //public Card(CardSO so)
        //{
        //    ID = so.ID;
        //    Name = so.Name;
        //    Mana = so.Mana;
        //    Marsion = so.Marsion;
        //    Attack = so.Attack;
        //    Health = so.Health;
        //}

        public string UID;
        public ulong ClientID { get; private set; }
        public int Rank { get; private set; }
        public Suit Suit { get; private set; }

        public Card(ulong clientID, CardSO so)
        {
            UID = Guid.NewGuid().ToString();
            ClientID = clientID;
            Rank = so.Rank;
            Suit = so.Suit;
        }

        public override bool Equals(object obj)
        {
            return obj is Card card &&
                   UID == card.UID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UID);
        }
    }
}