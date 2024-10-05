using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "NewStartTurnAbility", menuName = "Marsion/CardAbilities/Example")]
    public class StartTurnAbilityExample : CardAbility
    {
        public override void Register()
        {
            Managers.Server.OnTurnStarted += Activate;
        }

        public override void Activate()
        {
            Managers.Logger.Log<StartTurnAbilityExample>("Start Turn Marsion", colorName: "yellow");
        }

        public override void Clear()
        {
            Managers.Server.OnTurnStarted -= Activate;
        }
    }
}