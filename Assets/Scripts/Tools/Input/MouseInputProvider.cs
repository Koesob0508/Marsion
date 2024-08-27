using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    //[RequireComponent(typeof(Collider))]
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
            if(Camera.main.GetComponent<Physics2DRaycaster>() == null)
                throw new Exception(GetType() + " needs an " + typeof(Physics2DRaycaster) + " on the MainCamera");
        }

        #endregion

        #region IMouseInput

        Action<PointerEventData> IMouseInput.OnPointerEnter { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnPointerExit { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnPointerDown { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnPointerUp { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnPointerClick { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnBeginDrag { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnDrag { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnEndDrag { get; set; } = eventData => { };
        Action<PointerEventData> IMouseInput.OnDrop { get; set; } = eventData => { };

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Pointer Enter");
            ((IMouseInput)this).OnPointerEnter.Invoke(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Pointer Exit");
            ((IMouseInput)this).OnPointerExit.Invoke(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Pointer Down");
            ((IMouseInput)this).OnPointerDown.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Pointer Up");
            ((IMouseInput)this).OnPointerUp.Invoke(eventData);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Pointer Click");
            ((IMouseInput)this).OnPointerClick.Invoke(eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Begin Drag");
            ((IMouseInput)this).OnBeginDrag.Invoke(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Drag");
            ((IMouseInput)this).OnDrag.Invoke(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} End Drag");
            ((IMouseInput)this).OnEndDrag.Invoke(eventData);
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            Managers.Logger.LogPointer<MouseInputProvider>($"{gameObject.name} Drop");
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