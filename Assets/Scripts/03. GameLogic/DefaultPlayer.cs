using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Marsion
{
    /// <summary>
    /// 플레이어 클래스, 플레이어의 상태, 카드 위치 관리
    /// </summary>
    [Serializable]
    public class DefaultPlayer : IPlayer
    {
        [JsonProperty] public ulong ID { get; private set; }
        public string HeroID { get; }

        public ICard PlayerCard { get; private set; }
        [JsonProperty] public string Portrait { get; private set; }

        public List<ICard> Deck { get; private set; }
        public List<ICard> Hand { get; private set; }
        public List<ICard> Field { get; private set; }
        public List<ICard> Grave { get; private set; }

        public int Health => PlayerCard.Health;
        [JsonProperty] public int MaxHealth { get; private set; }

        [JsonProperty] public int Mana { get; private set; }
        [JsonProperty] public int MaxMana { get; private set; }

        public void Init()
        {
            PlayerCard = new Card();
            Hand = new();
            Field = new();

            PlayerCard.SetMaxHealth(MaxHealth);
            PlayerCard.Init(ID);
        }

        public void SetPlayerID(ulong playerID) { ID = playerID; }

        public void SetPlayerPortrait(string portraitID) { Portrait = portraitID; }

        public void SetDeck(List<ICard> deck) { Deck = deck; }

        public bool TryGetPlayerCard(string uid, out ICard card)
        {
            if(PlayerCard.UID == uid)
            {
                card = PlayerCard;

                return true;
            }
            else
            {
                card = null;

                return false;
            }
        }

        public bool TryGetHandCard(string uid, out ICard card)
        {
            foreach (ICard handCard in Hand)
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

        public bool TryGetFieldCard(string uid, out ICard card)
        {
            foreach (ICard fieldCard in Field)
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

        public override bool Equals(object obj)
        {
            return obj is DefaultPlayer player &&
                   ID == player.ID;
        }
    }
}