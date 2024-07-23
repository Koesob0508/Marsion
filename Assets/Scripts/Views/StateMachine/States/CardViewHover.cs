using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    public class CardViewHover : BaseCardViewState
    {
        #region Properites

        #endregion

        public CardViewHover(ICardView handler, BaseStateMachine fsm) : base(handler, fsm)
        {
        }

        #region State Operations

        public override void OnEnterState()
        {
            Handler.Input.OnPointerExit -= OnPointerExit;
            Handler.Input.OnPointerExit += OnPointerExit;

            Handler.Input.OnPointerClick -= OnPointerClick;
            Handler.Input.OnPointerClick += OnPointerClick;

            Handler.Input.OnDrag -= OnDrag;
            Handler.Input.OnDrag += OnDrag;

            SetScale();
            SetPosition();
            SetRotation();
        }

        public override void OnExitState()
        {
            Handler.Input.OnPointerExit -= OnPointerExit;
            Handler.Input.OnPointerClick -= OnPointerClick;
            Handler.Input.OnDrag -= OnDrag;
        }

        #endregion

        #region Pointer Operations

        private void OnPointerExit(PointerEventData eventData)
        {
            if (FSM.IsCurrent(this))
            {
                // 이전 상태가 Idle 상태이기 때문에 PopState
                FSM.PopState();
            }
        }

        private void OnPointerClick(PointerEventData eventData)
        {
            if (FSM.IsCurrent(this) && eventData.button == PointerEventData.InputButton.Left)
            {
                FSM.PopState();

                FSM.PushState<CardViewSelect>();
            }
        }

        private void OnDrag(PointerEventData eventData)
        {
            if (FSM.IsCurrent(this) && eventData.button == PointerEventData.InputButton.Left)
            {
                FSM.PopState();

                FSM.PushState<CardViewDrag>();
            }
        }

        #endregion

        #region Utils

        private void SetScale()
        {
            var currentScale = Handler.Transform.localScale;
            var finalScale = currentScale * 2;

            Handler.Transform.localScale = finalScale;
        }

        private void SetPosition()
        {
            var finalPosition = Handler.Transform.position + new Vector3(0, 0f, -2f);

            Handler.Transform.position = finalPosition;
        }

        private void SetRotation()
        {
            Handler.Transform.rotation = Quaternion.identity;
        }
        #endregion
    }
}