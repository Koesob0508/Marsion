using System;
using System.Collections;
using UnityEngine;

namespace Marsion.CardView
{
    public abstract class BaseCardViewMotion
    {
        #region Properties

        protected ICardView Handler { get; }
        public bool IsOperating { get; protected set; }
        public Action OnFinishMotion = () => { };
        protected virtual float Threshold => 0.01f;
        protected Vector3 TargetValue { get; set; }
        protected float Speed { get; set; }

        #endregion

        protected BaseCardViewMotion(ICardView handler) => Handler = handler;

        #region Operations

        public void Update()
        {
            if (!IsOperating) return;

            if (CheckFinalState())
                OnMotionEnds();
            else
                KeepMotion();
        }

        public virtual void Execute(Vector3 vector, float speed, float delay = 0, bool withZ = false)
        {
            Speed = speed;
            TargetValue = vector;

            if (delay == 0)
                IsOperating = true;
            else
                Handler.MonoBehaviour.StartCoroutine(AllowMotion(delay));
        }

        protected abstract bool CheckFinalState();

        protected virtual void OnMotionEnds() => OnFinishMotion?.Invoke();

        protected abstract void KeepMotion();

        IEnumerator AllowMotion(float delay)
        {
            yield return new WaitForSeconds(delay);
            IsOperating = true;
        }

        public virtual void StopMotion() => IsOperating = false;

        #endregion
    }
}