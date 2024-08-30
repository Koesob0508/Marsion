﻿
using Marsion.Logic;
using System;

namespace Marsion
{
    [Serializable]
    public class Card : IDamageable
    {
        public ulong ClientID { get; private set; }
        public string UID { get; private set; }
        public string Name { get; private set; }
        public int Mana { get; private set; }
        public string FullArtPath { get; private set; }
        public string BoardArtPath { get; private set; }
        public string AbilityExplain { get; private set; }
        public int Attack { get; private set; }
        public int HP { get; private set; }
        public bool IsDead { get; private set; }

        public Card(ulong clientID, CardSO so)
        {
            ClientID = clientID;
            UID = Guid.NewGuid().ToString();
            Name = so.Name;
            Mana = so.Mana;
            FullArtPath = so.FullArtPath;
            BoardArtPath = so.BoardArtPath;
            AbilityExplain = so.AbilityExplain;
            Attack = so.Attack;
            HP = so.Health;
            IsDead = false;
        }

        public void Damage(int amount)
        {
            HP -= amount;
        }

        public void Die()
        {
            Managers.Logger.Log<Card>($"{Name} Dead", colorName: "red");
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