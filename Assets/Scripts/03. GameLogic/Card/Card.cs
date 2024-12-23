
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
        public string UID { get; private set; }
        public ulong PlayerID { get; private set; }
        public string SOID { get; private set; }
        public string Name { get; private set; }
        public GradeType Grade { get; private set; }
        public int ManaCost { get; private set; }
        public string FullArtPath { get; private set; }
        public string BoardArtPath { get; private set; }
        public string AbilityExplain { get; private set; }
        public int Attack { get; private set; }
        public int MaxHP { get; private set; }
        public int HP { get; private set; }
        public bool IsDead { get; private set; }

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