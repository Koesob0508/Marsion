using UnityEngine;

namespace Marsion
{
    [System.Serializable]
    public enum AbilityType
    {
        Custom,
        Play,
        LastWill
    }

    [System.Serializable]
    public abstract class CardAbility : ScriptableObject
    {
        public Card Handler { get; set; }
        protected CardAbility(Card handler = null) => Handler = handler;

        [SerializeField]
        private AbilityType type;
        public AbilityType Type => type;

        [SerializeField]
        private string description;
        public string Description => description;

        public abstract void Register();

        public abstract void Activate();

        public abstract void Clear();
    }
}