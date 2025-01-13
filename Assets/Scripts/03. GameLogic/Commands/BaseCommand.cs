using Codice.CM.Common;
using UnityEngine;

namespace Marsion
{
    public abstract class BaseCommand : ICommand
    {
        public IGameLogic GameLogic { get; }
        public CommandData CommandData { get; }
        public TriggerType TriggerType { get; }

        public BaseCommand(IGameLogic logic, CommandData data, TriggerType trigger)
        {
            GameLogic = logic;
            CommandData = data;
            TriggerType = trigger;
        }

        public void Execute()
        {
            Logger.Log<BaseCommand>($"Execution Start", colorName: ColorCodes.Gray);
            Implement();
            Logger.Log<BaseCommand>($"Execution End", colorName: ColorCodes.Gray);
            CheckTrigger();
        }

        protected abstract void Implement();

        private void CheckTrigger()
        {
            Logger.Log<TriggerHandler>($"Trigger : {TriggerType}", colorName: ColorCodes.DeepPink);
            GameLogic.Trigger.Trigger(TriggerType);
        }
    }
}