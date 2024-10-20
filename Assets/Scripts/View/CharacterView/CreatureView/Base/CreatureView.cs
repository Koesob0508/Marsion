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

        public override void Init(Card card)
        {
            Transform = transform;
            Collider = GetComponent<Collider2D>();

            Input = GetComponent<IMouseInput>();
            Order = GetComponent<Order>();

            FSM = new CharacterViewFSM(this);

            if (IsEmpty) return;

            Managers.Client.Game.OnDataUpdated -= UpdateCard;
            Managers.Client.Game.OnDataUpdated += UpdateCard;

            Managers.Client.Game.OnStartAttack -= Attack;
            Managers.Client.Game.OnStartAttack += Attack;

            if (card == null)
                Managers.Logger.Log<CreatureView>("Card is null");

            Card = card;
            Text_Attack.text = card.Attack.ToString();
            Text_Health.text = card.Health.ToString();
            CardSprite.sprite = Managers.Resource.Load<Sprite>(card.BoardArtPath);
        }

        public override void Spawn()
        {
            FSM.PushState<CreatureViewSpawn>();
        }

        protected override void UpdateCard()
        {
            if (Card == null) Managers.Logger.Log<CreatureView>(gameObject.name, colorName: "yellow");
            Card = Managers.Client.Game.GetCard(Type, Card.PlayerID, Card.UID);
        }
    }
}