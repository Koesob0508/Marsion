using Marsion;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion.Logic
{
    [Serializable]
    public class Player
    {
        public ulong ClientID { get; private set; }

        public int Portrait;

        public List<Card> Deck = new List<Card>();
        public List<Card> Hand = new List<Card>();
        public List<Card> Field = new List<Card>();

        public Player(int clientID)
        {
            ClientID = (ulong)clientID;
        }

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
    }
}