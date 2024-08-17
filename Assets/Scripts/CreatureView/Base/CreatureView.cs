using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Marsion.CardView
{
    public class CreatureView : MonoBehaviour, ICreatureView
    {
        [SerializeField] TMP_Text Text_Attack;
        [SerializeField] TMP_Text Text_Health;
        [SerializeField] SpriteRenderer CardSprite;

        public Vector3 OriginPosition { get; set; }

        public Order Order => GetComponent<Order>();

        public void Setup(Card card)
        {
            if(card == null)
                Managers.Logger.Log<CreatureView>("Card is null");
            Text_Attack.text = card.Attack.ToString();
            Text_Health.text = card.Health.ToString();
            CardSprite.sprite = Managers.Resource.Load<Sprite>(card.BoardArtPath);
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