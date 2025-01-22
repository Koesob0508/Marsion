using Codice.CM.Common;
using UnityEngine;

namespace Marsion
{
    public abstract class BaseCommand : ICommand
    {
        public IGameLogic Logic { get; private set; }
        public BaseCommand(IGameLogic logic)
        {
            Logic = logic;
        }

        public void Execute()
        {
            Logger.Log<BaseCommand>($"Execution Start", colorName: ColorCodes.Gray);
            var eventData = Implement();
            Logger.Log<BaseCommand>($"Execution End", colorName: ColorCodes.Gray);
            TriggerEvent(eventData);
        }

        protected abstract EventData Implement();

        protected virtual void TriggerEvent(EventData data)
        {
            if (data.Type == EventType.None) return;
            Logic.Event.Trigger(data);
        }
    }
}