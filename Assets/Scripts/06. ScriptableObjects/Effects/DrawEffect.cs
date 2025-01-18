using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "DrawEffect", menuName = "Marsion/Effect/DrawEffect")]
    public class DrawEffect : EffectSO
    {
        public override void Execute(IGameLogic logic, EventData data)
        {
            logic.CommandHandler.Add(logic.CommandFactory.CreateDraw(data.PlayerID, data.IntValue));
        }
    }
}