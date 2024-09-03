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
            CardSprite.sprite = Managers.Resource.Load<Sprite>(card.BoardArtPath);
        }

        public override void Spawn()
        {
            FSM.PushState<CreatureViewSpawn>();
        }

        protected override void UpdateCard()
        {
            Card = Managers.Client.GetCard(Type, Card.PlayerID, Card.UID);
        }
    }
}