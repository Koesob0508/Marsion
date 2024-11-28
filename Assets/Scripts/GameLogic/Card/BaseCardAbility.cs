using System;
using UnityEngine;

namespace Marsion
{
    [Serializable]
    public enum AbilityType
    {
        Custom,
        Play,
        LastWill
    }

    public abstract class BaseCardAbility : ScriptableObject, ICardAbility
    {
        protected ICard Handler;

        public AbilityType Type;

        public string Description;

        public BaseCardAbility() { }

        protected BaseCardAbility(ICard handler = null) => Handler = handler;

        public abstract void Register();

        public abstract void Activate();

        public abstract void Clear();
    }
}