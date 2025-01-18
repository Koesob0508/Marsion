using Codice.CM.Common;
using System;

namespace Marsion
{
    public interface ICommand
    {
        EventData EventData { get; }
        IGameLogic Logic { get; }
        EventType EventType { get; }
        void Execute();
    }
}