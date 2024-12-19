using System;
using System.Collections.Generic;

namespace Marsion
{
    public class GameEventManager
    {
        private static readonly Dictionary<string, List<Action<object>>> _eventListeners = new();

        public static void RegisterEvent(string eventName, Action<object> listener)
        {
            if (!_eventListeners.ContainsKey(eventName))
                _eventListeners[eventName] = new List<Action<object>>();

            _eventListeners[eventName].Add(listener);
        }

        public static void UnregisterEvent(string eventName, Action<object> listener)
        {
            if (_eventListeners.ContainsKey(eventName))
                _eventListeners[eventName].Remove(listener);
        }

        public static void TriggerEvent(string eventName, object eventData)
        {
            if(_eventListeners.ContainsKey(eventName))
            {
                foreach (var listener in _eventListeners[eventName])
                    listener.Invoke(eventData);
            }
        }
    }
}