using UnityEngine;

namespace Marsion
{
    public interface IUIManager
    {
        T ShowPopupUI<T>(string name = null) where T : UI_Popup;
        T ShowSceneUI<T>(string name = null) where T : UI_Scene;
        T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base;
        void ClosePopupUI();
        void ClosePopupUI(UI_Popup popup);
        void CloseAllPopupUI();
        void SetCanvas(GameObject go, bool sort = true);
        void Clear();
    }
}