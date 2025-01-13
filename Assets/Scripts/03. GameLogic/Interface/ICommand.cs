using Codice.CM.Common;
using System;

namespace Marsion
{
    public interface ICommand
    {
        CommandData CommandData { get; }
        IGameLogic GameLogic { get; }
        TriggerType TriggerType { get; }
        void Execute();
    }
}