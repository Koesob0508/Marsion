using UnityEngine;

namespace Marsion.CardView
{
    public class ScaleCardViewMotion : BaseCardViewMotion
    {
        public ScaleCardViewMotion(ICardView handler) : base(handler) { }

        protected override bool CheckFinalState()
        {
            var delta = TargetValue - Handler.Transform.localScale;

            return delta.magnitude <= Threshold;
        }

        protected override void OnMotionEnds()
        {
            Handler.Transform.localScale = TargetValue;
            IsOperating = false;

            base.OnMotionEnds();
        }

        protected override void KeepMotion()
        {
            var current = Handler.Transform.localScale;
            var amount = Time.deltaTime * Speed;

            Handler.Transform.localScale = Vector3.Lerp(current, TargetValue, amount);
        }
    }
}