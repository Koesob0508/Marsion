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

            Managers.Instance.Client.Game.OnDataUpdated -= UpdateCard;
            Managers.Instance.Client.Game.OnDataUpdated += UpdateCard;

            Managers.Instance.Client.Game.OnAttackStarted -= Attack;
            Managers.Instance.Client.Game.OnAttackStarted += Attack;

            if (card == null)
                Logger.Log<CreatureView>("Card is null");

            Card = card;
            Text_Attack.text = card.Power.ToString();
            Text_Health.text = card.Health.ToString();
        }

        public virtual void Clear()
        {
            Managers.Instance.Client.Game.OnDataUpdated -= UpdateCard;
            Managers.Instance.Client.Game.OnAttackStarted -= Attack;
        }

        protected abstract void UpdateCard();

        public virtual void UpdateStatus()
        {
            Text_Attack.text = Card.Power.ToString();
            Text_Health.text = Card.Health.ToString();
        }

        public abstract void Spawn();
        public abstract void Die();
        protected virtual void Attack(Sequencer.Sequence sequence, DefaultPlayer attackPlayer, Card attacker, DefaultPlayer defendPlayer, Card defender)
        {
            if (Card.UID != attacker.UID) return;

            Sequencer.Clip startAttackClip = new("StartAttack", sequence, false);

            startAttackClip.OnPlay += () =>
            {
                FSM.AttackState.Target = Managers.Instance.Client.Game.GetCharacter(defendPlayer.PlayerID, defender.UID).MonoBehaviour.gameObject;
                FSM.AttackState.OnComplete += () =>
                {
                    startAttackClip.Complete();
                };

                FSM.PushState<CreatureViewAttack>();
            };
        }

        protected virtual void BeforeDead(Sequencer.Sequence sequence)
        {
            if (!Card.IsDead) return;

            Sequencer.Clip deadAnimClip = new Sequencer.Clip("DeadAnim", sequence, false);
            deadAnimClip.OnPlay += () =>
            {
                FSM.DeadState.OnComplete += () =>
                {
                    deadAnimClip.Complete();
                };

                FSM.PushState<CreatureViewDead>();
            };
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