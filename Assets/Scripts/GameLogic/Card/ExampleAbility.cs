using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "NewExampleAbility", menuName = "Marsion/CardAbilities/Example")]
    public class ExampleAbility : CardAbility
    {
        [SerializeField] string Log;
        public override void Register()
        {
            switch(Type)
            {
                case AbilityType.Play:
                    Handler.OnPlay += Activate;
                    break;
                case AbilityType.LastWill:
                    Handler.OnLastWill += Activate;
                    break;
            }
        }

        public override void Activate()
        {
            Debug.Log($"{Log}");
        }

        public override void Clear()
        {
            Handler.OnPlay -= Activate;
            Handler.OnLastWill -= Activate;
        }
    }
}