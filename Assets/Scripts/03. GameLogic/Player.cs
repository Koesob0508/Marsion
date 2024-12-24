using Marsion.Tool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Marsion
{
    /// <summary>
    /// 플레이어 클래스, 플레이어의 상태, 카드 위치 관리리
    /// </summary>
    [Serializable]
    public class Player
    {
        [JsonProperty] public ulong PlayerID { get; private set; }

        public Card PlayerCard;
        [JsonProperty] public string Portrait { get; private set; }

        public List<Card> Deck;
        public List<Card> Hand;
        public List<Card> Field;

        public int Health => PlayerCard.Health;
        [JsonProperty] public int MaxHealth { get; private set; }

        [JsonProperty] public int Mana { get; private set; }
        [JsonProperty] public int MaxMana { get; private set; }

        public void Init()
        {
            PlayerCard = new Card();
            Hand = new();
            Field = new();

            PlayerCard.SetPlayerID(PlayerID);
            PlayerCard.SetMaxHealth(MaxHealth);

            foreach(var card in Deck)
            {
                card.SetPlayerID(PlayerID);
                card.Init();
            }

            PlayerCard.Init();
        }

        public void SetPlayerID(ulong playerID) { PlayerID = playerID; }

        public void SetPlayerPortrait(string portraitID) { Portrait = portraitID; }

        public void SetPlayerDeck(List<Card> deck) { Deck = deck; }

        public bool TryGetHandCard(string uid, out Card card)
        {
            foreach (Card handCard in Hand)
            {
                if (handCard.UID == uid)
                {
                    card = handCard;
                    return true;
                }
            }

            card = null;
            return false;
        }

        public bool TryGetFieldCard(string uid, out Card card)
        {
            foreach (Card fieldCard in Field)
            {
                if (fieldCard.UID == uid)
                {
                    card = fieldCard;
                    return true;
                }
            }

            card = null;
            return false;
        }

        public void SetMaxHealth(int amount) { MaxHealth = amount; }

        public void SetMaxMana(int amount) { MaxMana = amount; }

        public void IncreaseMaxMana(int amount) { MaxMana += amount; }

        public void RestoreMana(int amount)
        {
            Mana += amount;
            if (Mana > MaxMana)
            {
                Mana = MaxMana;
            }
        }

        public void RestoreAllMana()
        {
            Mana = MaxMana;
        }

        public void PayMana(int amount)
        {
            Mana -= amount;
            if(Mana < 0)
            {
                Mana = 0;
            }
        }
    }
}