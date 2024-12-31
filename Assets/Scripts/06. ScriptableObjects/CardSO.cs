using UnityEngine;

namespace Marsion
{
    public enum GradeType
    {
        Basic = 0,
        Normal = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    [CreateAssetMenu(fileName = "CardSO", menuName = "Marsion/CardSO")]
    public class CardSO : ScriptableObject, IIdentifiable
    {
        [SerializeField] private string id;
        public string ID => id;
        public string Name;
        public int ManaCost;
        public GradeType Grade;
        public string FullArtPath;
        public string BoardArtPath;
        public string AbilityExplain;
        public int Attack;
        public int Health;
        public AbilitySO[] Abilities = new AbilitySO[0];

        public void SetID(string value) { id = value; }
    }
}