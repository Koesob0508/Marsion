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

        private IGameLogic Logic;
        private EventData Data;

        public void PrepareCast(IGameLogic gameLogic, EventData data)
        {
            Logic = gameLogic;
            Data = data;

            data.IntValue = value;
        }

        public void Execute()
        {
            Effect.Execute(Logic, Data);
        }
    }
}