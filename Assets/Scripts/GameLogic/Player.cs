﻿using Marsion;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;

namespace Marsion
{
    [Serializable]
    public class Player
    {
        public ulong ClientID;

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
    }
}