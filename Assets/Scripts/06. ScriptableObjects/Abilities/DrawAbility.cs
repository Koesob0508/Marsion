using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "DrawAbility", menuName = "Marsion/Ability/Draw")]
    public class DrawAbility : BaseAbility
    {
        public int Amount;

        public override void Execute(IGameLogic logic)
        {
            //logic.CommandHandler.Add(logic.CommandFactory.CreateDraw(data.SenderID, data.SenderID, Amount));
        }
    }
}