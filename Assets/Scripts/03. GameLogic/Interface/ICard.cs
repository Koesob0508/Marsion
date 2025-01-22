using System.Collections.Generic;

namespace Marsion
{
    public interface ICard
    {
        string SOID { get; }
        string UID { get; }
        ulong PlayerID { get; }
        string Name { get; }
        int Power { get; }
        int Health { get; }
        bool IsDead { get; }
        int ManaCost { get; }
        List<BaseAbility> CompositionAbilities { get; }
        List<BaseAbility> DecompositionAbilities { get; }
        List<BaseAbility> TriggerAbilities { get; }

        // TODO 아래 함수 리팩토링
        void Init(ulong playerID);
        void SetMaxHealth(int amount);
        void Kill();
        void CastCompositionAbility();
        void RegisterDecompositionAbility(BaseAbility ability);
        void CastDecompositionAbility();
        void RegisterAllTriggerAbility();
        void RegisterTriggerAbility(BaseAbility ability);
        void CastTriggerAbility();
    }
}