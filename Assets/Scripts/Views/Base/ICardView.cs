using UnityEngine;

namespace Marsion
{
    public interface ICardView : IFsmHandler
    {
        #region Properties

        CardViewFsm FSM { get; }
        Transform Transform { get; }
        IMouseInput Input { get; }

        #endregion

        #region Operations

        void Enable();

        #endregion
    }
}