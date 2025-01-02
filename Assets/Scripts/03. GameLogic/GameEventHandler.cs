using System;
using System.Collections.Generic;

namespace Marsion
{
    public class GameEventHandler
    {
        private readonly Dictionary<string, List<Action<string, GameCommandData>>> _eventListeners = new();
        private event Action<string, GameCommandData> _observerAlert;

        public void RegisterEvent(string eventName, Action<string, GameCommandData> listener)
        {
            if (!_eventListeners.ContainsKey(eventName))
                _eventListeners[eventName] = new List<Action<string, GameCommandData>>();

            _eventListeners[eventName].Add(listener);
        }

        public void UnregisterEvent(string eventName, Action<string, GameCommandData> listener)
        {
            if (_eventListeners.ContainsKey(eventName))
                _eventListeners[eventName].Remove(listener);
        }

        public void TriggerEvent(string eventName, GameCommandData eventData)
        {
            if (_eventListeners.ContainsKey(eventName))
            {
                foreach (var listener in _eventListeners[eventName])
                    listener.Invoke(eventName, eventData);

                _observerAlert?.Invoke(eventName, eventData);
            }
        }

        public void SubscribeEvent(Action<string, GameCommandData> observerAlert)
        {
            _observerAlert = observerAlert;
        }
    }
}