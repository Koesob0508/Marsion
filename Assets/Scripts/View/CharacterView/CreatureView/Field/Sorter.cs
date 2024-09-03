using DG.Tweening;
using UnityEngine;

namespace Marsion.CardView
{
    public class Sorter : MonoBehaviour
    {
        public void Sort(ICreatureView[] cards)
        {
            float targetY = gameObject.transform.position.y;

            for(int i = 0; i < cards.Length; i++)
            {
                float targetX = (cards.Length - 1) * -1.125f + i * 2.25f;
                var targetCard = cards[i];

                if (!targetCard.FSM.IsCurrent<CreatureViewIdle>()) continue;

                targetCard.Transform.DOMove(new Vector3(targetX, targetY, 0f), 0.5f);
                targetCard.Order.SetOriginOrder(i);
            }
        }
    }
}