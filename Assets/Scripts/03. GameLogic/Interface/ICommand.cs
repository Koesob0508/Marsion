using System;

namespace Marsion
{
    public interface ICommand
    {
        event Action<GameCommandData> OnCompleted;
        void Execute();
        void Clear();
    }
}