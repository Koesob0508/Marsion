using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "DeckSO", menuName = "Marsion/DeckSO")]
    public class DeckSO : ScriptableObject
    {
        public string title;

        public CardSO[] cards;
    }
}