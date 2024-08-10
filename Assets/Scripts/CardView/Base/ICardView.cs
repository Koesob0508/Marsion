using UnityEngine;

namespace Marsion
{
    public interface ICardView : IFsmHandler, ICardViewTransformMotion
    {
        #region Properties

        bool IsMine { get; }
        MonoBehaviour MonoBehaviour { get; }
        CardViewFsm FSM { get; }
        Transform Transform { get; }
        Collider Collider { get; }
        IMouseInput Input { get; }
        Order Order { get; }
        GameObject FrontImage { get; }
        GameObject BackImage { get; }

        #endregion

        #region Operations

        void Enable();

        void Draw();

        #endregion
    }
}