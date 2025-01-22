using UnityEngine;

namespace Marsion
{
    public enum AbilityType
    {
        Trigger,
        Composition,
        Decomposition,
    }

    public abstract class BaseAbility : ScriptableObject, IIdentifiable
    {
        [SerializeField] private string id;
        public string ID => id;
        public BaseEffect Effect;

        public abstract void Execute(IGameLogic logic);
    }
}