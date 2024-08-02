namespace Marsion
{
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

        public int Rank { get; private set; }
        public Suit Suit { get; private set; }

        public Card(CardSO so)
        {
            Rank = so.Rank;
            Suit = so.Suit;
        }
    }
}