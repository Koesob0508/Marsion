using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "NewStartTurnAbility", menuName = "Marsion/CardAbilities/Example")]
    public class StartTurnAbilityExample : BaseCardAbility
    {
        public override void Register()
        {
            Managers.Server.Game.OnTurnStarted += Activate;
        }

        public override void Activate()
        {
            Logger.Log<StartTurnAbilityExample>("Start Turn Marsion", colorName: "yellow");
        }

        public override void Clear()
        {
            Managers.Server.Game.OnTurnStarted -= Activate;
        }
    }
}