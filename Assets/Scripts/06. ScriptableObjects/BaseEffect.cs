using UnityEngine;

namespace Marsion
{
    public abstract class BaseEffect : ScriptableObject
    {
        public abstract void Execute(IGameLogic logic);
    }
}