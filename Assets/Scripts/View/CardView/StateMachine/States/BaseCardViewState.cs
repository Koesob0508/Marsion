using System;

namespace Marsion.CardView
{
    public abstract class BaseCardViewState : IState
    {
        public bool IsInitialized { get; }
        protected ICardView Handler { get; }
        protected BaseStateMachine FSM { get; }
        protected CardViewParameters Parameters { get; }
        public event Action OnComplete;

        protected BaseCardViewState(ICardView handler, BaseStateMachine fsm, CardViewParameters parameters)
        {
            Handler = handler;
            FSM = fsm;
            Parameters = parameters;

            IsInitialized = true;
        }

        public virtual void OnInitialize() { }

        public virtual void OnEnterState() { }

        public virtual void OnExitState() { }

        public virtual void OnUpdate() { }

        public virtual void OnClear() { }

        public virtual void OnNextState(IState Next) { }

        protected void Complete()
        {
            OnComplete?.Invoke();
            OnComplete = null;
        }
    }
}