using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "EffectSO", menuName = "Marsion/EffectSO")]
    public class EffectSO : ScriptableObject
    {
        public virtual void Init(int value)
        {

        }

        public virtual void Execute(IGameLogic logic, Card card)
        {

        }
    }
}