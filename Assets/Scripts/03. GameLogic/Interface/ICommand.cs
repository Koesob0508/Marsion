using System;

namespace Marsion
{
    public interface ICommand
    {
        event Action<LogicCommandData> OnCompleted;
        void Execute();
        void Clear();
    }
}