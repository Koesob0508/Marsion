using System;

namespace Marsion
{
    public interface IState
    {
        event Action OnComplete;
        bool IsInitialized { get; }
        void OnInitialize();
        void OnEnterState();
        void OnUpdate();
        void OnExitState();
        void OnClear();
        void OnNextState(IState Next);
    }
}