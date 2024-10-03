
using Marsion.Logic;
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

    [Serializable]
    public class Card : IDamageable
    {
        public ulong PlayerID { get; private set; }
        public string UID { get; private set; }
        public string Name { get; private set; }
        public GradeType Grade { get; private set; }
        public int Mana { get; private set; }
        public string FullArtPath { get; private set; }
        public string BoardArtPath { get; private set; }
        public string AbilityExplain { get; private set; }
        public int Attack { get; private set; }
        public int Health { get; private set; }
        public bool IsDead { get; private set; }
        public List<CardAbility> Abilities { get; private set; }

        public Action OnPlay;
        public Action OnLastWill;

        public Card(ulong playerID)
        {
            PlayerID = playerID;
            UID = Guid.NewGuid().ToString();
            IsDead = false;
        }

        public Card(ulong playerID, CardSO so)
        {
            PlayerID = playerID;
            UID = Guid.NewGuid().ToString();
            Name = so.Name;
            Grade = so.Grade;
            Mana = so.Mana;
            FullArtPath = so.FullArtPath;
            BoardArtPath = so.BoardArtPath;
            AbilityExplain = so.AbilityExplain;
            Attack = so.Attack;
            Health = so.Health;
            IsDead = false;
            Abilities = so.Abilities;
        }

        public void SetHP(int amount)
        {
            Health = amount;
        }

        public void Damage(int amount)
        {
            Health -= amount;
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