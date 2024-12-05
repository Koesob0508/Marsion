using UnityEngine;

namespace Marsion
{
    public interface IUIManager
    {
        T ShowUI<T>(string path = null, bool isPopup = false) where T : UI_Base;
        T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base;
        void ClosePopupUI();
        void ClosePopupUI(UI_Popup popup);
        void CloseAllPopupUI();
        void Clear();
    }
}