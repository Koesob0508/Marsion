
using Marsion.Logic;
using Newtonsoft.Json;
using System;

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
        //public List<CardAbility> Abilities { get; private set; }

        [JsonIgnore] public Action OnPlay;
        [JsonIgnore] public Action OnLastWill;

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
            //Abilities = so.Abilities;
        }

        [JsonConstructor]
        public Card(ulong playerID, string uID, string name, GradeType grade, int mana, string fullArtPath, string boardArtPath, string abilityExplain, int attack, int health, bool isDead) : this(playerID)
        {
            UID = uID;
            Name = name;
            Grade = grade;
            Mana = mana;
            FullArtPath = fullArtPath;
            BoardArtPath = boardArtPath;
            AbilityExplain = abilityExplain;
            Attack = attack;
            Health = health;
            IsDead = isDead;
        }

        // 깊은 복사를 위한 생성자 추가
        public Card(Card original)
        {
            PlayerID = original.PlayerID;
            UID = original.UID; // UID는 고유 식별자로 복사
            Name = original.Name;
            Grade = original.Grade;
            Mana = original.Mana;
            FullArtPath = original.FullArtPath;
            BoardArtPath = original.BoardArtPath;
            AbilityExplain = original.AbilityExplain;
            Attack = original.Attack;
            Health = original.Health;
            IsDead = original.IsDead;

            // Abilities를 깊은 복사하려면 Abilities 리스트가 필요함
            // Abilities = original.Abilities.Select(ability => new CardAbility(ability)).ToList(); // CardAbility 클래스에 복사 생성자 필요
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