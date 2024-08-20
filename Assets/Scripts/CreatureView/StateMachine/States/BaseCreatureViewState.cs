namespace Marsion.CardView
{
    public class BaseCreatureViewState : IState
    {
        public bool IsInitialized { get; }
        protected ICreatureView Handler { get; }
        protected CreatureViewFSM FSM { get; }

        protected BaseCreatureViewState(ICreatureView handler, CreatureViewFSM fsm)
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