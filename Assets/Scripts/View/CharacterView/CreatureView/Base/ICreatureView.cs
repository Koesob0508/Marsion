using UnityEngine;

namespace Marsion.CardView
{
    public interface ICreatureView : ICharacterView
    {
        Vector3 OriginPosition { get; set; }
        void Spawn();
        void MoveTransform(Vector3 position, bool useDOTween, float dotweenTime = 0);
    }
}