using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Marsion.CardView
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(IMouseInput))]
    public class CreatureView : MonoBehaviour, ICreatureView
    {

        #region UI Properties

        [SerializeField] TMP_Text Text_Attack;
        [SerializeField] TMP_Text Text_Health;
        [SerializeField] SpriteRenderer CardSprite;

        #endregion

        public Vector3 OriginPosition { get; set; }
        public Card Card { get; private set; }
        public MonoBehaviour MonoBehaviour => this;
        public CreatureViewFSM FSM { get; private set; }
        public Transform Transform { get; private set; }
        public Collider2D Collider { get; private set; }
        public IMouseInput Input { get; private set; }
        public Order Order { get; private set; }
        public string Name => gameObject.name;

        #region Unity Callbacks

        private void Start()
        {
            Managers.Logger.Log<CreatureView>("Start");
            Transform = transform;
            Collider = GetComponent<Collider2D>();

            Input = GetComponent<IMouseInput>();
            Order = GetComponent<Order>();

            FSM = new CreatureViewFSM(this);
        }

        private void Update()
        {
            FSM?.Update();
        }

        #endregion

        public void Setup(Card card)
        {
            if(card == null)
                Managers.Logger.Log<CreatureView>("Card is null");
            Text_Attack.text = card.Attack.ToString();
            Text_Health.text = card.Health.ToString();
            CardSprite.sprite = Managers.Resource.Load<Sprite>(card.BoardArtPath);
        }

        public void Spawn() => FSM.PushState<CreatureViewSpawn>();

        public void MoveTransform(Vector3 position, bool useDOTween, float dotweenTime = 0)
        {
            if (useDOTween)
                transform.DOMove(position, dotweenTime);
            else
                transform.position = position;
        }

        public override bool Equals(object obj)
        {
            return obj is CreatureView view &&
                   base.Equals(obj) &&
                   EqualityComparer<Card>.Default.Equals(Card, view.Card);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Card);
        }
    }
}