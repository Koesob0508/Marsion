using UnityEngine;

namespace Marsion
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
            var distance = TargetValue - Handler.Transform.position;

            if (!WithZ)
                distance.z = 0;

            return distance.magnitude <= Threshold;
        }

        protected override void OnMotionEnds()
        {
            WithZ = false;
            IsOperating = false;

            var targetValue = TargetValue;
            targetValue.z = Handler.Transform.position.z;
            Handler.Transform.position = targetValue;

            base.OnMotionEnds();
        }

        protected override void KeepMotion()
        {
            var current = Handler.Transform.position;
            var amount = Speed * Time.deltaTime;
            var delta = Vector3.Lerp(current, TargetValue, amount);

            if (!WithZ)
                delta.z = Handler.Transform.position.z;

            Handler.Transform.position = delta;
        }
    }
}