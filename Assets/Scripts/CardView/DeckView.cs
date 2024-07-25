using UnityEngine;

namespace Marsion
{
    
    public class DeckView : MonoBehaviour
    {
        public CardView cardPrefab;
        public HandView hand;
        private int count;

        [Button]
        public void DrawCard()
        {
            var cardObject = Instantiate(cardPrefab);
            cardObject.name = $"Card_{count}";
            hand.AddCard(cardObject);
            count++;
        }
    }
}