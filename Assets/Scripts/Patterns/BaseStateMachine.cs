using System;
using System.Collections.Generic;

namespace Marsion
{
    public abstract class BaseStateMachine
    {
        #region Constructor

        public IFSMHandler Handler { get; set; }
        protected BaseStateMachine(IFSMHandler handler = null) => Handler = handler;

        #endregion

        #region Properties

        public bool IsInitialized { get; protected set; }

        readonly Stack<IState> stack = new Stack<IState>();

        readonly Dictionary<Type, IState> register = new Dictionary<Type, IState>();

        public IState Current => PeekState();

        #endregion

        #region Initialize

        public void RegisterState(IState state)
        {
            if (state == null)
                throw new ArgumentNullException("Null is not a valid state");

            var type = state.GetType();
            register.Add(type, state);
            Managers.Logger.Log<BaseStateMachine>($"{Handler.Name}, Registered : ", "black", type);
        }

        public void Initialize()
        {
            OnBeforeInitialize();

            foreach (var state in register.Values)
                state.OnInitialize();

            IsInitialized = true;

            OnAfterInitialize();

            Managers.Logger.Log<BaseStateMachine>($"{Handler.Name}, Initialized! ", "yellow");
        }

        protected virtual void OnBeforeInitialize() { }

        protected virtual void OnAfterInitialize() { }

        #endregion

        #region Operations

        public void Update() => Current?.OnUpdate();

        public void PushState<T>(bool isSilent = false) where T : IState
        {
            var stateType = typeof(T);
            var state = register[stateType];

            PushState(state, isSilent);
        }

        public void PushState(IState state, bool isSilent = false)
        {
            var type = state.GetType();

            if (!register.ContainsKey(type))
                throw new ArgumentException($"State {state} not registered yet.");
            
            if (stack.Count > 0 && !isSilent)
                Current?.OnExitState();

            stack.Push(state);
            state.OnEnterState();

            Managers.Logger.Log<BaseStateMachine>($"{Handler.Name}, {stack.Count}, Push state : ", "green", type);
        }

        public void PopState(bool isSilent = false)
        {
            if (Current == null) return;

            var state = stack.Pop();

            Managers.Logger.Log<BaseStateMachine>($"{Handler.Name}, {stack.Count}, Pop state : ", "purple", state.GetType());
            state.OnExitState();

            if (!isSilent)
            {
                Current?.OnEnterState();
                Managers.Logger.Log<BaseStateMachine>($"Current State : ", "purple", Current.GetType());
            }
        }

        public void ReserverState()
        {
            
        }

        public virtual void Clear()
        {
            foreach (var state in register.Values)
                state.OnClear();

            stack.Clear();
            register.Clear();
        }

        #endregion

        #region Utils

        public bool IsCurrent<T>() where T : IState => Current?.GetType() == typeof(T);

        public bool IsCurrent(IState state)
        {
            if (state == null)
                throw new ArgumentNullException("Input state can not be null.");

            return Current?.GetType() == state.GetType();
        }

        public IState PeekState() => stack.Count > 0 ? stack.Peek() : null;

        #endregion
    }
}