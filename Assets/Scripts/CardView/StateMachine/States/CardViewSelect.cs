using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    public class CardViewSelect : BaseCardViewState
    {
        #region Fields

        private Plane plane;
        private Vector3 mousePos;

        #endregion

        public CardViewSelect(ICardView handler, BaseStateMachine fsm, CardViewParameters parameters) : base(handler, fsm, parameters) { }

        #region State Operations

        public override void OnEnterState()
        {
            Handler.Input.OnPointerDown -= OnPointerDown;
            Handler.Input.OnPointerDown += OnPointerDown;
        }

        public override void OnUpdate() => FollowCursor();

        public override void OnExitState()
        {
            Handler.Input.OnPointerDown -= OnPointerDown;
        }

        #endregion

        #region Pointer Operations

        private void OnPointerDown(PointerEventData eventData)
        {
            if(FSM.IsCurrent(this))
            {
                switch(eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        // Play!
                        break;

                    case PointerEventData.InputButton.Right:
                        // Cancel!
                        FSM.PopState();
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion

        #region Utils

        private void FollowCursor()
        {
            plane = new Plane(-Vector3.forward, Handler.Transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Handler.Input.MousePosition);
            if (plane.Raycast(ray, out float enter))
                mousePos = ray.GetPoint(enter);

            Handler.Transform.position = mousePos;
        }

        #endregion
    }
}