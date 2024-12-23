
using Marsion.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Marsion
{
    public enum GradeType
    {
        Basic = 0,
        Normal = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    /// <summary>
    /// 카드 클래스, 카드의 행동, 상태태 관리
    /// </summary>
    public class Card : IDamageable
    {
        [JsonProperty] public string UID { get; private set; }
        [JsonProperty] public ulong PlayerID { get; private set; }
        [JsonProperty] public string SOID { get; private set; }
        [JsonProperty] public string Name { get; private set; }
        [JsonProperty] public GradeType Grade { get; private set; }
        [JsonProperty] public int ManaCost { get; private set; }
        [JsonProperty] public string FullArtPath { get; private set; }
        [JsonProperty] public string BoardArtPath { get; private set; }
        [JsonProperty] public string AbilityExplain { get; private set; }
        [JsonProperty] public int Attack { get; private set; }
        [JsonProperty] public int MaxHP { get; private set; }
        [JsonProperty] public int HP { get; private set; }
        [JsonProperty] public bool IsDead { get; private set; }

        public void Init(ulong playerID)
        {
            UID = Guid.NewGuid().ToString();
            PlayerID = playerID;
            SOID = null;

            IsDead = false;
        }

        public void Init(ulong playerID, string soID)
        {
            UID = Guid.NewGuid().ToString();
            PlayerID = playerID;
            SOID = soID;

            IsDead = false;
        }

        // public void AddTrigger(ITrigger trigger)
        // {
        //     Triggers.Add(trigger);
        //     trigger.Register();
        // }

        // public void RemoveTrigger(ITrigger trigger)
        // {
        //     trigger.Unregister();
        //     Triggers.Remove(trigger);
        // }

        public void SetMaxHP(int amount)
        {
            MaxHP = amount;
        }

        public void SetHP(int amount)
        {
            HP = amount;
        }

        public void TakeDamage(int amount)
        {
            HP -= amount;
        }

        public void Die()
        {
            IsDead = true;
        }

        public override bool Equals(object obj)
        {
            return obj is Card card &&
                   UID == card.UID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UID);
        }
    }
}