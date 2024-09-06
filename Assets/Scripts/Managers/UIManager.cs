using Marsion.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class UIManager
    {
        int _order = 10;

        Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
        UI_Scene _sceneUI = null;

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

            GameObject go = Managers.Resource.Instantiate($"Prefabs/UI/Scene/{name}");

            T scene = go.GetOrAddComponent<T>();
            _sceneUI = scene;

            go.transform.SetParent(Root.transform);

            return scene;
        }

        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            Managers.Logger.Log<UIManager>("Show popup");

            if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

            GameObject go = Managers.Resource.Instantiate($"Prefabs/UI/Popup/{name}");
            T popup = go.GetOrAddComponent<T>();
            _popupStack.Push(popup);

            go.transform.SetParent(Root.transform);
            return popup;
        }

        public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
        {
            if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

            GameObject go = Managers.Resource.Instantiate($"Prefabs/UI/SubItem/{name}");

            if (parent != null) { go.transform.SetParent(parent); }

            return go.GetOrAddComponent<T>();
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0) return;

            UI_Popup popup = _popupStack.Pop();
            Managers.Resource.Destroy(popup.gameObject);
            popup = null;
            _order--;
        }

        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupStack.Count == 0) return;

            if (_popupStack.Peek() != popup)
            {
                Debug.LogWarning("Close Popup Failed");
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