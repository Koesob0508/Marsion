using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    [RequireComponent(typeof(IMouseInput))]
    public abstract class BaseDropZone : MonoBehaviour
    {
        protected IMouseInput Input { get; set; }
        
        protected virtual void Awake()
        {
            Input = GetComponent<IMouseInput>();
            Input.OnPointerUp += OnPointerUp;
        }

        protected virtual void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}