using DG.Tweening;
using Marsion.Tool;
using System;
using TMPro;
using UnityEngine;

namespace Marsion.CardView
{
    public enum CardType
    {
        Hero,
        Field
    }

    public abstract class CharacterView : MonoBehaviour, ICharacterView
    {
        #region UI Properties

        [SerializeField] protected TMP_Text Text_Attack;
        [SerializeField] protected TMP_Text Text_Health;
        [SerializeField] protected SpriteRenderer CardSprite;

        #endregion

        [SerializeField] protected Pointer pointer;
        [SerializeField] protected CardType Type;

        public Vector3 OriginPosition { get; set; }
        public Card Card { get; protected set; }
        public MonoBehaviour MonoBehaviour => this;
        public CharacterViewFSM FSM { get; protected set; }
        public Transform Transform { get; protected set; }
        public Collider2D Collider { get; protected set; }
        public IMouseInput Input { get; protected set; }
        public Order Order { get; protected set; }
        public Pointer Pointer => pointer;
        public string Name => gameObject.name;

        #region Unity Callbacks

        private void Update()
        {
            FSM?.Update();
        }

        #endregion

        public virtual void Init(Card card)
        {
            Transform = transform;
            Collider = GetComponent<Collider2D>();

            Input = GetComponent<IMouseInput>();
            Order = GetComponent<Order>();

            FSM = new CharacterViewFSM(this);

            Managers.Client.OnDataUpdated -= UpdateCard;
            Managers.Client.OnDataUpdated += UpdateCard;

            Managers.Client.OnStartAttack -= Attack;
            Managers.Client.OnStartAttack += Attack;

            Managers.Client.OnCharacterBeforeDead -= BeforeDead;
            Managers.Client.OnCharacterBeforeDead += BeforeDead;

            if (card == null)
                Managers.Logger.Log<CreatureView>("Card is null");

            Card = card;
            Text_Attack.text = card.Attack.ToString();
            Text_Health.text = card.HP.ToString();
        }

        public virtual void Clear()
        {
            Managers.Client.OnDataUpdated -= UpdateCard;
            Managers.Client.OnStartAttack -= Attack;
            Managers.Client.OnCharacterBeforeDead -= BeforeDead;
        }

        protected abstract void UpdateCard();

        public virtual void UpdateStatus()
        {
            Text_Attack.text = Card.Attack.ToString();
            Text_Health.text = Card.HP.ToString();
        }

        public abstract void Spawn();

        protected virtual void Attack(MyTween.Sequence sequence, Player attackPlayer, Card attacker, Player defendPlayer, Card defender)
        {
            if (Card.UID != attacker.UID) return;

            MyTween.Task attackTask = new();

            attackTask.Action = () =>
            {
                FSM.Target = Managers.Client.GetCreature(defendPlayer.ClientID, defender.UID).MonoBehaviour.gameObject;
                FSM.PushState<CreatureViewAttack>(attackTask.OnComplete);
            };

            sequence.Append(attackTask);
        }

        protected virtual void BeforeDead(MyTween.Sequence sequence)
        {
            if (!Card.IsDead) return;

            MyTween.Task deadAction = new MyTween.Task();
            deadAction.Action = () =>
            {
                FSM.PushState<CreatureViewDead>(deadAction.OnComplete);
            };

            sequence.Join(deadAction);
        }

        public override bool Equals(object obj)
        {
            return obj is CreatureView view &&
                   Card.Equals(view.Card);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Card);
        }
    }
}