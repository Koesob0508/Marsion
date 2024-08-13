using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion.CardView
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

            Handler.Order.SetMostFrontOrder(true);
            Handler.Transform.rotation = Quaternion.identity;
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
            if (FSM.IsCurrent(this))
            {
                if(eventData.button == PointerEventData.InputButton.Left && DetectArea() && Managers.Client.TryPlayCard(Handler.Card))
                {
                    Managers.Client.PlayCard(Handler.Card);
                    Handler.MonoBehaviour.gameObject.SetActive(false);
                }

                FSM.PopState();
            }
        }

        #endregion

        #region Utils

        private void FollowCursor()
        {
            Handler.Transform.position = GetMouseWorldPosition();
        }

        private bool DetectArea()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(GetMouseWorldPosition(), Vector3.forward);

            int layer = LayerMask.NameToLayer("PlayArea");
            return Array.Exists(hits, x => x.collider.gameObject.layer == layer);
        }

        private Vector3 GetMouseWorldPosition()
        {
            plane = new Plane(-Vector3.forward, Handler.Transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Handler.Input.MousePosition);

            if (plane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            else
            {
                Managers.Logger.Log<CardViewSelect>("Point not found.");
                return default;
            }
        }

        #endregion
    }
}