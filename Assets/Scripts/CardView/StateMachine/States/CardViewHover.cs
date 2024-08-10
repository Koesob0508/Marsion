using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    public class CardViewHover : BaseCardViewState
    {
        #region Properites

        #endregion

        public CardViewHover(ICardView handler, BaseStateMachine fsm, CardViewParameters parameters) : base(handler, fsm, parameters) { }

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
            SetOrder();
        }

        public override void OnExitState()
        {
            Handler.HoverImage.transform.localPosition = Vector3.zero;
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
            var finalScale = currentScale * Parameters.HoverScale;

            Handler.HoverImage.transform.localScale = finalScale;
        }

        private void SetPosition()
        {
            var finalPosition = Handler.Transform.position + new Vector3(0, Parameters.HoverHeight, -2f);

            //Handler.MoveToWithZ(finalPosition, Parameters.HoverSpeed);
            Handler.HoverImage.transform.position = finalPosition;
        }

        private void SetRotation()
        {
            Handler.Rotation.StopMotion();
            Handler.Transform.rotation = Quaternion.identity;
        }

        private void SetOrder()
        {
            Handler.Order.SetMostFrontOrder(true);
        }

        #endregion
    }
}