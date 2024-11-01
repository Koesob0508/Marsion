using Marsion.Logic;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "CardSO", menuName = "Marsion/CardSO")]
    public class CardSO : ScriptableObject, IIdentifiable
    {
        [SerializeField] private string id;
        public string ID => id;
        public string Name;
        public int Mana;
        public GradeType Grade;
        public string FullArtPath;
        public string BoardArtPath;
        public string AbilityExplain;
        public int Attack;
        public int Health;
        public List<CardAbility> Abilities;
    }
}