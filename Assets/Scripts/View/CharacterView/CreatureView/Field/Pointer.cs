using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Marsion.CardView
{
    public class Pointer : MonoBehaviour
    {
        Plane plane;
        public IMouseInput Input { get; private set; }
        public UnityAction<bool, GameObject> OnClick;
        public string LayerName { get; set; }

        private void Start()
        {
            Input = GetComponent<IMouseInput>();

            Input.OnPointerUp -= OnPointeUp;
            Input.OnPointerUp += OnPointeUp;
        }

        private void Update()
        {
            transform.position = GetMouseWorldPosition();
        }

        private void OnPointeUp(PointerEventData eventData)
        {
            GameObject foundObject = null;
            bool result = false;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                result = IsAreaDetected(LayerName, out foundObject);
            }

            OnClick?.Invoke(result, foundObject);
            LayerName = null;
            OnClick = null;

            gameObject.SetActive(false);
        }

        private bool IsAreaDetected(string name, out GameObject detectedObject)
        {
            // �ʱ�ȭ
            detectedObject = null;

            // ��� RaycastHit2D ������ ������
            RaycastHit2D[] hits = Physics2D.RaycastAll(GetMouseWorldPosition(), Vector3.forward);

            // �־��� �̸��� ���̾ ������
            int layer = LayerMask.NameToLayer(name);

            // �ش� ���̾��� GameObject�� ã��
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.layer == layer)
                {
                    detectedObject = hit.collider.gameObject;
                    return true;
                }
            }

            return false;
        }

        private Vector3 GetMouseWorldPosition()
        {
            plane = new Plane(-Vector3.forward, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.MousePosition);

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
    }
}