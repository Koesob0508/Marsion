using Codice.CM.Common;
using System;

namespace Marsion
{
    public interface ICommand
    {
        IGameLogic GameLogic { get; }
        ICondition Condition { get; }
        TriggerType Trigger { get; }
        void Register();
        void Unregister();
        void Execute(CommandData data);
        void CheckTrigger();
    }
}