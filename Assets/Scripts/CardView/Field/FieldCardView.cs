using DG.Tweening;
using UnityEngine;
using Card = Marsion.Logic.Card;

namespace Marsion.CardView
{
    public class FieldCardView : MonoBehaviour, IFieldCardView
    {
        public Vector3 OriginPosition { get; set; }

        public Order Order => GetComponent<Order>();

        public void Setup(Card card)
        {
            Managers.Logger.Log<FieldCardView>("Setup");
        }

        public void MoveTransform(Vector3 position, bool useDOTween, float dotweenTime = 0)
        {
            if (useDOTween)
                transform.DOMove(position, dotweenTime);
            else
                transform.position = position;
        }
    }
}