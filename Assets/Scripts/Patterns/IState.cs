using System;

namespace Marsion
{
    public interface IState
    {
        Action OnComplete { get; set; }
        bool IsInitialized { get; }
        void OnInitialize();
        void OnEnterState();
        void OnUpdate();
        void OnExitState();
        void OnClear();
        void OnNextState(IState Next);
    }
}