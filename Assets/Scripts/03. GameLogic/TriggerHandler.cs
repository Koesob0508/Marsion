using System;
using System.Collections.Generic;

namespace Marsion
{
    public class TriggerHandler : ITriggerHandler
    {
        private readonly Dictionary<TriggerType, List<Action<CommandData>>> _triggerListeners = new();

        public void Register(TriggerType type, Action<CommandData> listener)
        {
            if (!_triggerListeners.ContainsKey(type))
                _triggerListeners[type] = new List<Action<CommandData>>();

            _triggerListeners[type].Add(listener);
        }

        public void Unregister(TriggerType type, Action<CommandData> listener)
        {
            if (_triggerListeners.ContainsKey(type))
                _triggerListeners[type].Remove(listener);
        }

        public void Trigger(TriggerType type, CommandData eventData = null)
        {
            if (_triggerListeners.ContainsKey(type))
            {
                foreach (var listener in _triggerListeners[type])
                    listener.Invoke(eventData);
            }
        }
    }
}