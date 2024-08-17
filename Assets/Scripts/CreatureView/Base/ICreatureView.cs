using UnityEngine;

namespace Marsion.CardView
{
    public interface ICreatureView
    {
        Vector3 OriginPosition { get; set; }
        Order Order { get; }

        void MoveTransform(Vector3 position, bool useDOTween, float dotweenTime = 0);
    }
}