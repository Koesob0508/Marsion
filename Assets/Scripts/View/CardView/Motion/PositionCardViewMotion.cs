using UnityEngine;

namespace Marsion.CardView
{
    public class PositionCardViewMotion : BaseCardViewMotion
    {
        bool WithZ { get; set; }

        public PositionCardViewMotion(ICardView card) : base(card) { }

        public override void Execute(Vector3 vector, float speed, float delay = 0, bool withZ = false)
        {
            WithZ = withZ;

            base.Execute(vector, speed, delay, withZ);
        }

        protected override bool CheckFinalState()
        {
            var distance = TargetValue - Handler.Transform.localPosition;

            if (!WithZ)
                distance.z = 0;

            return distance.magnitude <= Threshold;
        }

        protected override void OnMotionEnds()
        {
            WithZ = false;
            IsOperating = false;

            var targetValue = TargetValue;
            targetValue.z = Handler.Transform.localPosition.z;
            Handler.Transform.localPosition = targetValue;

            base.OnMotionEnds();
        }

        protected override void KeepMotion()
        {
            var current = Handler.Transform.localPosition;
            var amount = Speed * Time.deltaTime;
            var delta = Vector3.Lerp(current, TargetValue, amount);

            if (!WithZ)
                delta.z = Handler.Transform.localPosition.z;

            Handler.Transform.localPosition = delta;
        }
    }
}