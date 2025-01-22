using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "DrawEffect", menuName = "Marsion/Effect/DrawEffect")]
    public class DrawEffect : BaseEffect
    {
        public override void Execute(IGameLogic logic)
        {
            //logic.CommandHandler.Add(logic.CommandFactory.CreateDraw(drawData.SenderID, drawData.TargetPlayerID, drawData.Count));
        }
    }
}