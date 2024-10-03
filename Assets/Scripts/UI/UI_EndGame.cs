using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion.UI
{
    public class UI_EndGame : UI_Popup
    {
        public TMP_Text Text_Result;

        IMouseInput Input;

        public override void Init()
        {
            base.Init();

            Input = GetComponent<IMouseInput>();

            Input.OnPointerClick += OnClick;
        }

        public override void ClosePopupUI()
        {
            base.ClosePopupUI();
        }

        private void OnClick(PointerEventData eventData)
        {
            ClosePopupUI();

            Managers.UI.ShowPopupUI<UI_DeckBuilder>();
        }
    }
}