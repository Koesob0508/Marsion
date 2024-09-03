using Marsion.Tool;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Marsion
{
    [Serializable]
    public class Player
    {
        public ulong ClientID { get; private set; }

        public Card Card;
        public int Portrait;

        public MyDictionary<string, Card> Cards = new MyDictionary<string, Card>();
        public List<Card> Deck = new List<Card>();
        public List<Card> Hand = new List<Card>();
        public List<Card> Field = new List<Card>();
        public List<Card> Proecssing = new List<Card>();

        public Player(int clientID)
        {
            ClientID = (ulong)clientID;
            Card = new Card(ClientID);
        }

        public string LogPile(List<Card> pile)
        {
            string result = "";

            Managers.Logger.Log<Player>("Log pile");

            return result;
        }

        public Card GetCard(string uid)
        {
            Cards.Add(Card.UID, Card);
            
            foreach (Card card in Hand)
            {
                Cards.Add(card.UID, card);
            }

            foreach (Card card in Field)
            {
                Cards.Add(card.UID, card);
            }

            Cards.TryGetValue(uid, out var result);
            Cards.Clear();

            return result;
        }
        
        public Card GetCard(List<Card> pile, string uid)
        {
            Card result = null;

            foreach (Card card in pile)
            {
                if (card.UID == uid)
                {
                    result = card;
                    break;
                }
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is Player player &&
                   ClientID == player.ClientID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientID);
        }
    }
}