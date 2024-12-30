using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "DrawEffect", menuName = "Marsion/Effect/DrawEffect")]
    public class DrawEffect : EffectSO
    {
        int value;

        public override void Init(int value)
        {
            this.value = value;
        }

        public override void Execute(IGameLogic logic, Card card)
        {
            logic.DrawCard(card.PlayerID, value);
        }
    }
}