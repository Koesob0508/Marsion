using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "EffectSO", menuName = "Marsion/EffectSO")]
    public abstract class EffectSO : ScriptableObject
    {
        public TriggerType TriggerType;

        public abstract void Init(int value);
        public abstract void Execute(IGameLogic logic, ICard card);
    }
}