using System;
using System.Collections.Generic;

namespace Marsion
{
    public class EventHandler : IEventHandler
    {
        private readonly Dictionary<EventType, List<Action<EventData>>> _eventListeners = new();

        public void Register(EventType type, Action<EventData> listener)
        {
            if (!_eventListeners.ContainsKey(type))
                _eventListeners[type] = new List<Action<EventData>>();

            _eventListeners[type].Add(listener);
        }

        public void Unregister(EventType type, Action<EventData> listener)
        {
            if (_eventListeners.ContainsKey(type))
                _eventListeners[type].Remove(listener);
        }

        public void Trigger(EventData eventData)
        {
            Logger.Log<EventHandler>($"Try trigger event : {eventData.Type}", colorName: ColorCodes.DeepPink);
            if (_eventListeners.ContainsKey(eventData.Type))
            {
                Logger.Log<EventHandler>($"Trigger event : {eventData.Type}", colorName: ColorCodes.DarkGray);
                foreach (var listener in _eventListeners[eventData.Type])
                    listener.Invoke(eventData);

                Logger.Log<EventHandler>($"Trigger event end", colorName:ColorCodes.DarkGray);
            }
        }
    }
}