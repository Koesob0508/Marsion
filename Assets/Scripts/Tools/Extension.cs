using Marsion.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion
{
    public static class Extension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return Util.GetOrAddComponent<T>(go);
        }

        public static void BindEvent(this GameObject go, Action<PointerEventData> action, UI_Event type = UI_Event.Click)
        {
            UI_Base.BindEvent(go, action, type);
        }
    }
}