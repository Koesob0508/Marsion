using Codice.CM.Common;

namespace Marsion
{
    public class BaseCommand : ICommand
    {
        public IGameLogic GameLogic => throw new System.NotImplementedException();
        public ICondition Condition => throw new System.NotImplementedException();
        public TriggerType Trigger => throw new System.NotImplementedException();

        public virtual void Execute()
        {

        }

        public virtual void Execute(CommandData data)
        {
            CheckTrigger();
        }

        public void Register()
        {
            GameLogic.Trigger.RegisterTrigger(Trigger, Execute);
        }

        public void Unregister()
        {
            GameLogic.Trigger.UnregisterTrigger(Trigger, Execute);
        }

        public void CheckTrigger()
        {
            GameLogic.Trigger.TriggerEvent(Trigger);
        }
    }
}