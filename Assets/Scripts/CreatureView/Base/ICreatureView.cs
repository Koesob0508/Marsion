using UnityEngine;

namespace Marsion.CardView
{
    public interface ICreatureView : IFSMHandler
    {
        Vector3 OriginPosition { get; set; }
        Card Card { get; }
        MonoBehaviour MonoBehaviour { get; }
        CreatureViewFSM FSM { get; }
        Transform Transform { get; }
        Collider2D Collider { get; }
        IMouseInput Input { get; }
        Order Order { get; }

        void Setup(Card card);

        #region State Operations

        void Spawn();

        #endregion

        void MoveTransform(Vector3 position, bool useDOTween, float dotweenTime = 0);
    }
}