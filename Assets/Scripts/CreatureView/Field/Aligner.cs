using UnityEngine;

namespace Marsion.CardView
{
    public class Aligner : MonoBehaviour
    {
        public void Align(ICreatureView[] cards)
        {
            float targetY = gameObject.transform.position.y;

            for(int i = 0; i < cards.Length; i++)
            {
                float targetX = (cards.Length - 1) * -1.125f + i * 2.25f;
                var targetCard = cards[i];

                targetCard.OriginPosition = new Vector3(targetX, targetY, 0f);
                targetCard.MoveTransform(targetCard.OriginPosition, true, 0.5f);
                targetCard.Order.SetOriginOrder(i);
            }
        }
    }
}