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
    public class CreatureView : CharacterView, ICreatureView
    {
        [SerializeField] bool IsEmpty;
        IFieldView Field;

        public void Init(Card card, IFieldView field)
        {
            Field = field;

            Transform = transform;
            Collider = GetComponent<Collider2D>();

            Input = GetComponent<IMouseInput>();
            Order = GetComponent<Order>();

            FSM = new CharacterViewFSM(this);

            if (IsEmpty) return;

            Managers.Client.Game.OnDataUpdated -= UpdateCard;
            Managers.Client.Game.OnDataUpdated += UpdateCard;

            Managers.Client.Game.OnAttackStarted -= Attack;
            Managers.Client.Game.OnAttackStarted += Attack;

            if (card == null)
                Logger.Log<CreatureView>("Card is null");

            Card = card;

            Managers.Instance.Data.GetDictionary<CardSO>().TryGetValue(Card.SOID, out var cardSO);

            Text_Attack.text = cardSO.Attack.ToString();
            Text_Health.text = cardSO.Health.ToString();
            CardSprite.sprite = Managers.Instance.Resource.Load<Sprite>(cardSO.BoardArtPath);
        }

        public override void Spawn()
        {
            FSM.PushState<CreatureViewSpawn>();
        }

        public override void Die()
        {
            FSM.DeadState.OnComplete += () =>
            {
                Field.Remove(this);
                Logger.Log<CreatureView>("Remove this", colorName: ColorCodes.Yellow);
            };

            FSM.PushState<CreatureViewDead>();
        }

        protected override void UpdateCard()
        {
            if (Card.IsDead) return;
            Card = Managers.Client.Game.GetCard(Type, Card.PlayerID, Card.UID);
        }
    }
}