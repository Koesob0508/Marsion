using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class UIManager : IUIManager
    {
        private readonly IResourceManager _resourceManager;
        private readonly CanvasOrderHandler _orderManager;

        private UI_Scene _sceneUI = null;
        private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

        public UIManager(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
            _orderManager = new CanvasOrderHandler();
        }

        public T ShowUI<T>(string path = null, bool isPopup = false) where T : UI_Base
        {
            if (string.IsNullOrEmpty(path))
            {
                if(isPopup)
                {
                    path = $"Prefabs/UI/Popup/{typeof(T).Name}";
                }
                else
                { 
                    path = $"Prefabs/UI/Scene/{typeof(T).Name}";
                }
            }

            GameObject go = _resourceManager.Instantiate(path);
            if (go == null)
            {
                Debug.LogWarning($"Failed to load Popup UI: {path}");
                return null;
            }

            T ui = go.GetOrAddComponent<T>();

            go.transform.SetParent(GetUIRoot().transform);

            if(isPopup)
            {
                _popupStack.Push(ui as UI_Popup);
            }
            else
            {
                _sceneUI = ui as UI_Scene;
            }

            _orderManager.SetCanvas(go, isPopup);

            return ui;
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0) return;

            UI_Popup popup = _popupStack.Pop();
            _resourceManager.Destroy(popup.gameObject);
            _orderManager.DecrementOrder();
        }

        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupStack.Count == 0 || _popupStack.Peek() != popup)
            {
                Debug.LogWarning("Close Popup Failed");
                return;
            }

            ClosePopupUI();
        }

        public void CloseAllPopupUI()
        {
            while (_popupStack.Count > 0) ClosePopupUI();
        }

        public void Clear()
        {
            CloseAllPopupUI();
            _sceneUI = null;
        }

        public GameObject GetUIRoot()
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }

        public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
        {
            if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

            GameObject go = Managers.Instance.Resource.Instantiate($"Prefabs/UI/SubItem/{name}");

            if (parent != null) { go.transform.SetParent(parent); }

            return go.GetOrAddComponent<T>();
        }

        
    }
}