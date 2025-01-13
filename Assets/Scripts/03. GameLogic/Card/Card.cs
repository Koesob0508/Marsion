
using Marsion.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Marsion
{
    /// <summary>
    ///     카드 클래스, 카드의 행동, 상태 관리
    /// </summary>
    public class Card : IDamageable
    {
        [JsonProperty] public string UID { get; private set; }
        [JsonProperty] public ulong PlayerID { get; private set; }
        [JsonProperty] public string SOID { get; private set; }
        [JsonProperty] public string Name { get; private set; }
        [JsonProperty] public int ManaCost { get; private set; }
        [JsonProperty] public int Attack { get; private set; }
        [JsonProperty] public int MaxHealth { get; private set; }
        [JsonProperty] public int Health { get; private set; }
        [JsonProperty] public bool IsDead { get; private set; }

        private IGameLogic _logic;
        private List<AbilitySO> SpellAbilities;
        private List<AbilitySO> Abilities;
        public void Init(ulong playerID)
        {
            UID = Guid.NewGuid().ToString();

            PlayerID = playerID;
            SOID = string.Empty;
            Name = string.Empty;
            ManaCost = 0;
            Attack = 0;
            Health = MaxHealth;

            IsDead = false;
        }

        // 생물 카드용 초기화
        public void Init(IGameLogic gameLogic, ulong playerID, CardSO cardSO)
        {
            UID = Guid.NewGuid().ToString();

            PlayerID = playerID;
            SOID = cardSO.ID;
            Name = cardSO.Name;
            ManaCost = cardSO.ManaCost;
            Attack = cardSO.Attack;
            MaxHealth = cardSO.Health;

            Health = MaxHealth;
            IsDead = false;

            _logic = gameLogic;
            SpellAbilities = new();
            Abilities = new();

            foreach (var ability in cardSO.Abilities)
            {
                if(ability.Type == AbilityType.Spell)
                {
                    SpellAbilities.Add(ability);
                }
                else
                {
                    Abilities.Add(ability);
                }

                ability.Init();
            }
        }

        public List<AbilitySO> GetCreatureSpell()
        {
            return SpellAbilities;
        }

        public void SetPlayerID(ulong playerID) { PlayerID = playerID; }
        public void SetMaxHealth(int amount) { MaxHealth = amount; }

        public void SetHealth(int amount)
        {
            Health = amount;
        }

        public void TakeDamage(int amount)
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