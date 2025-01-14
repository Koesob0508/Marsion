using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "BuffAttack", menuName = "Marsion/Effect/BuffAttack")]
    public class BuffAttack : EffectSO
    {
        private int _value;

        public override void Init(int value)
        {
            _value = value;
        }

        public override void Execute(IGameLogic logic, ICard card)
        {
            Debug.Log($"{card.Name} 공격력 {_value} 증가!");
        }
    }
}