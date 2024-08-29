using System;

namespace Marsion.CardView
{
    public abstract class BaseCardViewState : IState
    {
        #region Fields and Properties

        public bool IsInitialized { get; }
        protected ICardView Handler { get; }
        protected BaseStateMachine FSM { get; }
        protected CardViewParameters Parameters { get; }
        public Action OnComplete { get; set; }

        #endregion

        #region Constructor

        protected BaseCardViewState(ICardView handler, BaseStateMachine fsm, CardViewParameters parameters)
        {
            Handler = handler;
            FSM = fsm;
            Parameters = parameters;

            IsInitialized = true;
        }

        #endregion

        #region Utils

        //protected void EnableCollision() => Handler.Collider.enabled = true;

        //protected void DisableCollision() => Handler.Collider.enabled = false;

        #endregion

        /// <summary>
        ///     각 State의 필요에 따라 메서드 구현
        /// </summary>
        #region IState

        public virtual void OnInitialize() { }

        public virtual void OnEnterState() { }

        public virtual void OnExitState()
        {
            OnComplete?.Invoke();
            OnComplete= null;
        }

        public virtual void OnUpdate() { }

        public virtual void OnClear() { }

        public virtual void OnNextState(IState Next) { }

        #endregion
    }
}