using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    public class CardViewIdle : BaseCardViewState
    {
        Vector3 DefaultSize { get; }

        public CardViewIdle(ICardView handler, BaseStateMachine fsm) : base(handler, fsm)
        {
            DefaultSize = Handler.Transform.localScale;
        }

        #region State Operations

        public override void OnEnterState()
        {
            Handler.Input.OnPointerEnter -= OnPointerEnter;
            Handler.Input.OnPointerEnter += OnPointerEnter;

            Handler.Transform.localScale = DefaultSize;
        }

        public override void OnExitState()
        {
            Handler.Input.OnPointerEnter -= OnPointerEnter;
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
    }
}