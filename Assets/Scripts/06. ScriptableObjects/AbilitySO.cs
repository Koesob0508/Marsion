using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "AbilitySO", menuName = "Marsion/AbilitySO")]
    public class AbilitySO : ScriptableObject, IIdentifiable
    {
        [SerializeField] private string id;
        public string ID => id;
        public int value;
        public EffectSO[] Effects;

        public virtual void Init()
        {
            foreach(var effect in Effects)
            {
                effect.Init(value);
            }
        }

        public virtual void Execute(IGameLogic gameLogic, Card card)
        {
            foreach(var effect in Effects)
            {
                effect.Execute(gameLogic, card);
            }
        }
    }
}