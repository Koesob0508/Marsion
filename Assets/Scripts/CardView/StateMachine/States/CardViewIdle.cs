using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    public class CardViewIdle : BaseCardViewState
    {
        Vector3 DefaultSize { get; }

        public CardViewIdle(ICardView handler, BaseStateMachine fsm, CardViewParameters parameters) : base(handler, fsm, parameters)
        {
            DefaultSize = Handler.FrontImage.transform.localScale;
        }

        #region State Operations

        public override void OnEnterState()
        {
            Handler.Input.OnPointerEnter -= OnPointerEnter;
            Handler.Input.OnPointerEnter += OnPointerEnter;

            if (Handler.Position.IsOperating)
            {
                Handler.Collider.enabled = false;
                Handler.Position.OnFinishMotion -= OnCollider;
                Handler.Position.OnFinishMotion += OnCollider;
            }
            else
            {
                OnCollider();
            }

            Handler.FrontImage.transform.localScale = DefaultSize;
            Handler.Order.SetMostFrontOrder(false);
        }

        public override void OnExitState()
        {
            Handler.Input.OnPointerEnter -= OnPointerEnter;

            Handler.Position.OnFinishMotion -= OnCollider;
        }

        #endregion

        #region Pointer Operations

        private void OnPointerEnter(PointerEventData eventData)
        {
            if(FSM.IsCurrent(this))
            {
                // Hover 상태로 전환!
                FSM.PushState<CardViewHover>();
            }
        }

        #endregion

        private void OnCollider()
        {
            Handler.Collider.enabled = true;
        }
    }
}