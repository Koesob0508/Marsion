using Marsion.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class UIManager : IUIManager
    {
        private readonly IResourceManager _resourceManager;

        int _order = 10;

        UI_Scene _sceneUI = null;
        Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

        public UIManager(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find("@UI_Root");
                if (root == null)
                    root = new GameObject { name = "@UI_Root" };

                return root;
            }
        }

        public void SetCanvas(GameObject go, bool sort = true)
        {
            Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            if(sort)
            {
                canvas.sortingOrder = _order;
                _order++;
            }
            else
            {
                canvas.sortingOrder = 0;
            }
        }

        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

            GameObject go = _resourceManager.Instantiate($"Prefabs/UI/Scene/{name}");
            T scene = go.GetOrAddComponent<T>();
            go.transform.SetParent(Root.transform);

            _sceneUI = scene;

            return scene;
        }

        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

            GameObject go = _resourceManager.Instantiate($"Prefabs/UI/Popup/{name}");
            T popup = go.GetOrAddComponent<T>();
            go.transform.SetParent(Root.transform);

            _popupStack.Push(popup);

            return popup;
        }

        public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
        {
            if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

            GameObject go = _resourceManager.Instantiate($"Prefabs/UI/SubItem/{name}");

            if (parent != null) { go.transform.SetParent(parent); }

            return go.GetOrAddComponent<T>();
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0) return;

            UI_Popup popup = _popupStack.Pop();
            _resourceManager.Destroy(popup.gameObject);
            _order--;
        }

        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupStack.Count == 0) return;
            if (_popupStack.Peek() != popup)
            {
                Logger.LogWarning<UIManager>("Close Popup Failed");
                return;
            }

            ClosePopupUI();
        }

        public void CloseAllPopupUI()
        {
            while (_popupStack.Count > 0) { ClosePopupUI(); }
        }

        public void Clear()
        {
            CloseAllPopupUI();
            _sceneUI = null;
        }
    }
}