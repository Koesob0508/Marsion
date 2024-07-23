using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    [RequireComponent(typeof(Collider))]
    public class MouseInputProvider : MonoBehaviour, IMouseInput
    {
        #region Fields and Properties

        private Vector3 oldPosition;
        public Vector2 MousePosition => Input.mousePosition;
        public DragDirection DragDirection => GetDragDirection();

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if(Camera.main.GetComponent<PhysicsRaycaster>() == null)
                throw new Exception(GetType() + " needs an " + typeof(PhysicsRaycaster) + " on the MainCamera");
        }

        #endregion

        #region IMouseInput

        Action<PointerEventData> IMouseInput.OnPointerEnter { get; set; } = eventData => { Debug.Log("OnPointerEnter"); };
        Action<PointerEventData> IMouseInput.OnPointerExit { get; set; } = eventData => { Debug.Log("OnPointerExit"); };
        Action<PointerEventData> IMouseInput.OnPointerDown { get; set; } = eventData => { Debug.Log("OnPointerDown"); };
        Action<PointerEventData> IMouseInput.OnPointerUp { get; set; } = eventData => { Debug.Log("OnPointerUp"); };
        Action<PointerEventData> IMouseInput.OnPointerClick { get; set; } = eventData => { Debug.Log("OnPointerClick"); };
        Action<PointerEventData> IMouseInput.OnBeginDrag { get; set; } = eventData => { Debug.Log("OnBeginDrag"); };
        Action<PointerEventData> IMouseInput.OnDrag { get; set; } = eventData => { Debug.Log("OnDrag"); };
        Action<PointerEventData> IMouseInput.OnEndDrag { get; set; } = eventData => { Debug.Log("OnEndDrag"); };
        Action<PointerEventData> IMouseInput.OnDrop { get; set; } = eventData => { Debug.Log("OnDrop"); };

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerEnter.Invoke(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerExit.Invoke(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerDown.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerUp.Invoke(eventData);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerClick.Invoke(eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            ((IMouseInput)this).OnBeginDrag.Invoke(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            ((IMouseInput)this).OnDrag.Invoke(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            ((IMouseInput)this).OnEndDrag.Invoke(eventData);
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            ((IMouseInput)this).OnDrop.Invoke(eventData);
        }

        #endregion

        #region Utils

        private DragDirection GetDragDirection()
        {
            var currentPosition = Input.mousePosition;
            var normalized = (currentPosition - oldPosition).normalized;

            oldPosition = currentPosition;

            if (normalized.x > 0) return DragDirection.Right;
            if (normalized.x < 0) return DragDirection.Left;
            if (normalized.y > 0) return DragDirection.Up;
            if (normalized.y < 0) return DragDirection.Down;

            return DragDirection.None;
        }

        #endregion
    }
}