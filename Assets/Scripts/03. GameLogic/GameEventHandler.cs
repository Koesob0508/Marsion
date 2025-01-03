using System;
using System.Collections.Generic;

namespace Marsion
{
    public class GameEventHandler
    {
        private readonly Dictionary<string, List<Action<GameCommandData>>> _eventListeners = new();
        private event Action<string, GameCommandData> _observerAlert;

        public void RegisterEvent(string eventName, Action<GameCommandData> listener)
        {
            if (!_eventListeners.ContainsKey(eventName))
                _eventListeners[eventName] = new List<Action<GameCommandData>>();

            _eventListeners[eventName].Add(listener);
        }

        public void UnregisterEvent(string eventName, Action<GameCommandData> listener)
        {
            if (_eventListeners.ContainsKey(eventName))
                _eventListeners[eventName].Remove(listener);
        }

        public void TriggerEvent(string eventName, GameCommandData eventData = null)
        {
            if (_eventListeners.ContainsKey(eventName))
            {
                foreach (var listener in _eventListeners[eventName])
                    listener.Invoke(eventData);

                _observerAlert?.Invoke(eventName, eventData);
            }
        }

        public void SubscribeEvent(Action<string, GameCommandData> observerAlert)
        {
            _observerAlert = observerAlert;
        }

        public void UnsubscribeEvent(Action<string, GameCommandData> observerAlert)
        {
            _observerAlert = null;
        }
    }
}