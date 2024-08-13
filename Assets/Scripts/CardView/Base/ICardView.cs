﻿using Card = Marsion.Logic.Card;
using UnityEngine;

namespace Marsion.CardView
{
    public interface ICardView : IFsmHandler, ICardViewTransformMotion
    {
        #region Properties

        Card Card { get; }
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