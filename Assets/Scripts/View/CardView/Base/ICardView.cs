using UnityEngine;

namespace Marsion.CardView
{
    public interface ICardView : IFSMHandler, ICardViewTransformMotion
    {
        #region Properties

        Card Card { get; }
        MonoBehaviour MonoBehaviour { get; }
        CardViewFsm FSM { get; }
        Transform Transform { get; }
        Collider2D Collider { get; }
        IMouseInput Input { get; }
        Order Order { get; }
        GameObject FrontImage { get; }
        GameObject BackImage { get; }

        #endregion

        #region Operations

        void Setup();

        void Enable();

        void Draw();

        #endregion
    }
}