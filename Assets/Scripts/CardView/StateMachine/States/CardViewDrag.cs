using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    public class CardViewDrag : BaseCardViewState
    {
        #region Fields

        private Plane plane;
        private Vector3 mousePos;

        #endregion

        public CardViewDrag(ICardView handler, BaseStateMachine fsm, CardViewParameters parameters) : base(handler, fsm, parameters) { }

        #region State Operations

        public override void OnEnterState()
        {
            Handler.Input.OnPointerUp -= OnPointerUp;
            Handler.Input.OnPointerUp += OnPointerUp;
        }

        public override void OnUpdate() => FollowCursor();

        public override void OnExitState()
        {
            Handler.Input.OnPointerUp -= OnPointerUp;
        }

        #endregion

        #region Pointer Operations

        private void OnPointerUp(PointerEventData eventData)
        {
            // Pointer가 Up 됐을 때,
            // Hand 영역이면 Cancel
            // Play 영역이면 Play
            // ...
            // 그걸 어떻게?

            // 지금 당장은 Cancel로
            FSM.PopState();
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