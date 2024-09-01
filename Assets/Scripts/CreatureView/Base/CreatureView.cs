using DG.Tweening;
using Marsion.Tool;
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
        [SerializeField] Pointer pointer;

        #endregion

        [SerializeField] bool IsEmpty;

        public Vector3 OriginPosition { get; set; }
        public Card Card { get; private set; }
        public MonoBehaviour MonoBehaviour => this;
        public CreatureViewFSM FSM { get; private set; }
        public Transform Transform { get; private set; }
        public Collider2D Collider { get; private set; }
        public IMouseInput Input { get; private set; }
        public Order Order { get; private set; }
        public Pointer Pointer => pointer;
        public string Name => gameObject.name;

        #region Unity Callbacks

        private void Awake()
        {
            Transform = transform;
            Collider = GetComponent<Collider2D>();

            Input = GetComponent<IMouseInput>();
            Order = GetComponent<Order>();

            FSM = new CreatureViewFSM(this);
        }

        private void Start()
        {
            if (IsEmpty) return;

            Managers.Client.OnDataUpdated -= UpdateCard;
            Managers.Client.OnDataUpdated += UpdateCard;

            Managers.Client.OnStartAttack -= Attack;
            Managers.Client.OnStartAttack += Attack;

            Managers.Client.OnCreatureBeforeDead -= CheckDead;
            Managers.Client.OnCreatureBeforeDead += CheckDead;
        }

        private void Update()
        {
            FSM?.Update();
        }

        #endregion

        public void Setup(Card card)
        {
            if (card == null)
                Managers.Logger.Log<CreatureView>("Card is null");

            Card = card;
            Text_Attack.text = card.Attack.ToString();
            Text_Health.text = card.HP.ToString();
            CardSprite.sprite = Managers.Resource.Load<Sprite>(card.BoardArtPath);
        }

        public void Spawn() => FSM.PushState<CreatureViewSpawn>();

        public void UpdateCard()
        {
            Card = Managers.Client.GetGameData().GetFieldCard(Card.PlayerID, Card.UID);
        }

        public void UpdateStatus()
        {
            Text_Attack.text = Card.Attack.ToString();
            Text_Health.text = Card.HP.ToString();
        }

        public void Attack(MyTween.Sequence sequence, Player attackPlayer, Card attacker, Player defendPlayer, Card defender)
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

        public void CheckDead(MyTween.Sequence sequence)
        {
            if (!Card.IsDead) return;

            MyTween.Task deadAction = new MyTween.Task();
            deadAction.Action = () =>
            {
                FSM.PushState<CreatureViewDead>();
            };

            sequence.Append(deadAction);
        }

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