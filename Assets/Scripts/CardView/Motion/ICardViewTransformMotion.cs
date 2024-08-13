using UnityEngine;

namespace Marsion.CardView
{
    public interface ICardViewTransformMotion
    {
        BaseCardViewMotion Scale { get; }

        BaseCardViewMotion Position { get; }

        BaseCardViewMotion Rotation { get; }

        void ScaleTo(Vector3 scale, float speed, float delay = 0);

        void MoveTo(Vector3 position, float speed, float delay = 0);

        void MoveToWithZ(Vector3 position, float speed, float delay = 0);

        void RotateTo(Vector3 euler, float speed, float delay = 0);
    }
}
