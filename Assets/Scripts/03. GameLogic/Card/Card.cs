
using Marsion.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marsion
{
    /// <summary>
    ///     카드 클래스, 카드의 행동, 상태 관리
    /// </summary>
    public class Card : ICard, IDamageable
    {
        public string UID { get; private set; }
        public ulong PlayerID { get; private set; }
        public string SOID { get; private set; }
        public string Name { get; private set; }
        public int ManaCost { get; private set; }
        public int Power { get; private set; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public bool IsDead { get; private set; }
        public List<BaseAbility> CompositionAbilities { get; private set; }
        public List<BaseAbility> DecompositionAbilities { get; private set; }
        public List<BaseAbility> TriggerAbilities { get; private set; }

        private IGameLogic _logic;
        public void Init(ulong playerID)
        {
            UID = Guid.NewGuid().ToString();

            PlayerID = playerID;
            SOID = string.Empty;
            Name = string.Empty;
            ManaCost = 0;
            Power = 0;
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
            Power = cardSO.Attack;
            MaxHealth = cardSO.Health;
            CompositionAbilities = cardSO.CompositionAbilities.ToList();
            DecompositionAbilities = cardSO.DecompositionAbilities.ToList();
            TriggerAbilities = cardSO.TriggerAbilities.ToList();

            Health = MaxHealth;
            IsDead = false;

            _logic = gameLogic;
        }

        public void SetMaxHealth(int amount) { MaxHealth = amount; }

        public void Kill()
        {
            IsDead = true;
            _logic.CommandHandler.Add(_logic.CommandFactory.CreateCheckDead());
        }

        public void CastCompositionAbility()
        {
            foreach(var ability in CompositionAbilities)
            {
                _logic.CommandHandler.Add(_logic.CommandFactory.CreateCastSpell(PlayerID, UID, ability, AbilityType.Composition));
            }
        }

        public void RegisterDecompositionAbility(BaseAbility ability)
        {
            DecompositionAbilities.Add(ability);
        }

        public void CastDecompositionAbility()
        {
            foreach(var ability in DecompositionAbilities)
            {
                _logic.CommandHandler.Add(_logic.CommandFactory.CreateCastSpell(PlayerID, UID, ability, AbilityType.Decomposition));
            }
        }

        public void RegisterAllTriggerAbility()
        {

        }

        public void RegisterTriggerAbility(BaseAbility ability)
        {

        }

        public void CastTriggerAbility()
        {

        }

        public void Die()
        {
            IsDead = true;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
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