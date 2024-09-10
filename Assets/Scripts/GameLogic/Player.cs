using Marsion.Tool;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

        public int Mana { get; private set; }
        public int MaxMana { get; private set; }

        public Player(int clientID)
        {
            ClientID = (ulong)clientID;
            Card = new Card(ClientID);
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

            if (result == null)
                Debug.LogWarning("Get card result is null.");

            return result;
        }

        public void IncreaseMaxMana(int amount)
        {
            MaxMana += amount;
        }

        public void RestoreMana(int amount)
        {
            Mana += amount;
        }

        public void RestoreAllMana()
        {
            Mana = MaxMana;
        }

        public void PayMana(int amount)
        {
            Mana -= amount;
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