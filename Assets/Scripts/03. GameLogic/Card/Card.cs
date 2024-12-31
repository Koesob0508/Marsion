
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
        private List<AbilitySO> _abilities;
        private List<ITrigger> _triggers;

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
            _triggers = new();
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
            _triggers = new();
            _abilities = new();

            foreach (var ability in cardSO.Abilities)
            {
                _abilities.Add(ability);
                ability.Init();
            }
        }

        public void ExecutePlayAbility()
        {
            foreach (var ability in _abilities)
            {
                if (ability.Type == AbilityType.Play)
                {
                    ability.Execute(_logic, this);
                }
            }
        }

        public void AddTrigger(ITrigger trigger)
        {
            _triggers.Add(trigger);
            trigger.Register();
        }

        public void RemoveTrigger(ITrigger trigger)
        {
            trigger.Unregister();
            _triggers.Remove(trigger);
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