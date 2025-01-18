using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "EffectSO", menuName = "Marsion/EffectSO")]
    public abstract class EffectSO : ScriptableObject
    {
        public abstract void Execute(IGameLogic logic, EventData data);
    }
}