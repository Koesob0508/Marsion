using Codice.CM.Common;
using UnityEngine;

namespace Marsion
{
    public abstract class BaseCommand : ICommand
    {
        public IGameLogic Logic { get; }
        public EventData EventData { get; protected set; }
        public EventType EventType { get; protected set; }

        public BaseCommand(IGameLogic logic, EventData data = null)
        {
            Logic = logic;
            EventData = data;
        }

        public BaseCommand(IGameLogic logic, EventData data, EventType @event)
        {
            Logic = logic;
            EventData = data;
            EventType = @event;
        }

        public void Execute()
        {
            Logger.Log<BaseCommand>($"Execution Start", colorName: ColorCodes.Gray);
            Implement();
            Logger.Log<BaseCommand>($"Execution End", colorName: ColorCodes.Gray);
            TriggerEvent();
        }

        protected abstract void Implement();

        protected virtual void TriggerEvent()
        {
            if (EventType == EventType.None) return;
            Logic.Event.Trigger(EventType, EventData);
        }
    }
}