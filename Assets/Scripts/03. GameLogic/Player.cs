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
        public ulong PlayerID { get; private set; }

        public Card Card;
        public string Portrait { get; private set; }

        public MyDictionary<string, Card> Cards = new MyDictionary<string, Card>();
        public List<Card> Deck = new List<Card>();
        public List<Card> Hand = new List<Card>();
        public List<Card> Field = new List<Card>();

        [JsonProperty]
        public int Mana { get; private set; }
        [JsonProperty]
        public int MaxMana { get; private set; }

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

        public void SetMaxHP(int amount)
        {
            Card.SetMaxHP(amount);
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

        public void SetPlayerPortrait(string portraitID)
        {
            Portrait = portraitID;
        }
    }
}