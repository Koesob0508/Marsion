using UnityEngine;

namespace Marsion
{
    public class RotationCardViewMotion : BaseCardViewMotion
    {
        public RotationCardViewMotion(ICardView handler) : base(handler) { }

        protected override float Threshold => 0.05f;

        protected override bool CheckFinalState()
        {
            var distance = TargetValue - Handler.Transform.eulerAngles;
            var smallerThenLimit = distance.magnitude <= Threshold;
            var equals360 = (int)distance.magnitude == 360;
            var isFinal = smallerThenLimit || equals360;

            return isFinal;
        }

        protected override void OnMotionEnds()
        {
            Handler.Transform.eulerAngles = TargetValue;
            IsOperating = false;
            OnFinishMotion?.Invoke();
        }

        protected override void KeepMotion()
        {
            var current = Handler.Transform.rotation;
            var amount = Speed * Time.deltaTime;
            var rotation = Quaternion.Euler(TargetValue);
            var newRotation = Quaternion.RotateTowards(current, rotation, amount);

            Handler.Transform.rotation = newRotation;
        }
    }
}