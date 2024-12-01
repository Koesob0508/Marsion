namespace Marsion
{
    public interface IUIManager
    {
        T ShowUI<T>(string path, bool isPopup = false) where T : UI_Base;
        void ClosePopupUI();
        void ClosePopupUI(UI_Popup popup);
        void CloseAllPopupUI();
        void Clear();
    }
}