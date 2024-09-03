using System;

namespace Marsion.CardView
{
    public class BaseCreatureViewState : IState
    {
        public bool IsInitialized { get; }
        protected ICharacterView Handler { get; }
        protected CharacterViewFSM FSM { get; }
        public Action OnComplete { get; set; }

        protected BaseCreatureViewState(ICharacterView handler, CharacterViewFSM fsm)
        {
            Handler = handler;
            FSM = fsm;

            IsInitialized = true;
        }

        public virtual void OnInitialize() { }

        public virtual void OnEnterState() { }

        public virtual void OnUpdate() { }

        public virtual void OnExitState() { }

        public virtual void OnClear() { }

        public virtual void OnNextState(IState Next) { }
    }
}