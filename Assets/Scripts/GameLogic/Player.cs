using Marsion.Tool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Marsion
{
    [Serializable]
    public class Player
    {
        public ulong PlayerID { get; private set; }

        public Card Card;
        public string Portrait;

        public MyDictionary<string, Card> Cards = new MyDictionary<string, Card>();
        public List<Card> Deck = new List<Card>();
        public List<Card> Hand = new List<Card>();
        public List<Card> Field = new List<Card>();

        [JsonProperty]
        public int Mana { get; private set; }
        [JsonProperty]
        public int MaxMana { get; private set; }

        public Player(int clientID)
        {
            PlayerID = (ulong)clientID;
            Card = new Card(PlayerID);
        }

        [JsonConstructor]
        public Player(ulong playerID)
        {
            PlayerID = playerID;
        }

        public Player(Player original)
        {
            PlayerID = original.PlayerID;
            Portrait = original.Portrait;
            Mana = original.Mana;
            MaxMana = original.MaxMana;

            // Card 복사 (Card 클래스에 복사 생성자 필요)
            Card = new Card(original.Card);

            // Cards 딕셔너리 깊은 복사
            Cards = new MyDictionary<string, Card>();
            foreach (var key in original.Cards.Keys)
            {
                original.Cards.TryGetValue(key, out var value);
                Cards.Add(key, new Card(value)); // Card 클래스에 복사 생성자 필요
            }

            // Deck, Hand, Field 리스트 깊은 복사
            Deck = new List<Card>(original.Deck.Count);
            foreach (var card in original.Deck)
            {
                Deck.Add(new Card(card)); // Card 클래스에 복사 생성자 필요
            }

            Hand = new List<Card>(original.Hand.Count);
            foreach (var card in original.Hand)
            {
                Hand.Add(new Card(card)); // Card 클래스에 복사 생성자 필요
            }

            Field = new List<Card>(original.Field.Count);
            foreach (var card in original.Field)
            {
                Field.Add(new Card(card)); // Card 클래스에 복사 생성자 필요
            }
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

        public void SetMaxMana(int amount)
        {
            MaxMana = amount;
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
                   PlayerID == player.PlayerID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerID);
        }
    }
}