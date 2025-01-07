using System;

namespace Marsion
{
    public enum TriggerType
    {
        Spawn,
    }

    public interface ITriggerHandler
    {
        void RegisterTrigger(TriggerType type, Action<CommandData> listener);
        void UnregisterTrigger(TriggerType type, Action<CommandData> listener);
        void TriggerEvent(TriggerType type, CommandData triggerData = null);
    }
}