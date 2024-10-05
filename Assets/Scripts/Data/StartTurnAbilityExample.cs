using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "NewStartTurnAbility", menuName = "Marsion/CardAbilities/Example")]
    public class StartTurnAbilityExample : CardAbility
    {
        public override void Register()
        {
            Managers.Server.GameServer.OnTurnStarted += Activate;
        }

        public override void Activate()
        {
            Managers.Logger.Log<StartTurnAbilityExample>("Start Turn Marsion", colorName: "yellow");
        }

        public override void Clear()
        {
            Managers.Server.GameServer.OnTurnStarted -= Activate;
        }
    }
}