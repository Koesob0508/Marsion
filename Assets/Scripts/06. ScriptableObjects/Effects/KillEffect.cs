using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "KillEffect", menuName = "Marsion/Effect/KillEffect")]
    public class KillEffect : BaseEffect
    {
        public override void Execute(IGameLogic logic)
        {
            //logic.CommandHandler.Add(logic.CommandFactory.CreateKillCreature(killData.SenderID, killData.SenderCardUID, killData.TargetPlayerID, killData.TargetCardUID));
        }
    }
}