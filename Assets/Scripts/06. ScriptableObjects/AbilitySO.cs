using System;
using UnityEngine;

namespace Marsion
{
    public enum AbilityType
    {
        Ongoing,
        Spell,
        Exit
    }

    [CreateAssetMenu(fileName = "AbilitySO", menuName = "Marsion/AbilitySO")]
    public class AbilitySO : ScriptableObject, IIdentifiable
    {
        [SerializeField] private string id;
        public string ID => id;
        public AbilityType Type;
        public int value;
        public EffectSO Effect;

        public virtual void Init()
        {
            try
            {
                Effect.Init(value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initializing Effect: {ex.Message}");
            }
        }
    }
}