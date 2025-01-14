using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "DrawEffect", menuName = "Marsion/Effect/DrawEffect")]
    public class DrawEffect : EffectSO
    {
        private int _value;

        public override void Init(int value)
        {
            _value = value;
        }
        public override void Execute(IGameLogic logic, ICard card)
        {
            logic.DrawCard(card.PlayerID, _value);
        }
    }
}