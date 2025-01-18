using System;
using System.Collections.Generic;

namespace Marsion
{
    public class EventHandler : IEventHandler
    {
        private readonly Dictionary<EventType, List<Action<EventData>>> _triggerListeners = new();

        public void Register(EventType type, Action<EventData> listener)
        {
            if (!_triggerListeners.ContainsKey(type))
                _triggerListeners[type] = new List<Action<EventData>>();

            _triggerListeners[type].Add(listener);
        }

        public void Unregister(EventType type, Action<EventData> listener)
        {
            if (_triggerListeners.ContainsKey(type))
                _triggerListeners[type].Remove(listener);
        }

        public void Trigger(EventType type, EventData eventData = null)
        {
            Logger.Log<EventHandler>($"Try trigger event : {type}", colorName: ColorCodes.DeepPink);
            if (_triggerListeners.ContainsKey(type))
            {
                Logger.Log<EventHandler>($"Trigger event : {type}", colorName: ColorCodes.DarkGray);
                foreach (var listener in _triggerListeners[type])
                    listener.Invoke(eventData);

                Logger.Log<EventHandler>($"Trigger event end", colorName:ColorCodes.DarkGray);
            }
        }
    }
}