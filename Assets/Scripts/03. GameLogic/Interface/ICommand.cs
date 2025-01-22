using Codice.CM.Common;
using System;

namespace Marsion
{
    public interface ICommand
    {
        IGameLogic Logic { get; }
        void Execute();
    }
}