using UnityEngine;

namespace Marsion
{
    public enum Suit
    {
        Spade,
        Heart,
        Diamond,
        Club
    }

    [CreateAssetMenu(fileName = "CardSO", menuName = "Marsion/CardSO")]
    public class CardSO : ScriptableObject
    {
        

        //public int ID;
        //public string Name;
        //public Sprite Sprite;
        //public int Mana;
        //public int Marsion;
        //public int Attack;
        //public int Health;
        //public string Explanation;
        //public string VanilaText;

        public int Rank;
        public Suit Suit;
    }
}