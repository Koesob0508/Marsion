using UnityEngine;

namespace Marsion.CardView
{
    public class CardViewDraw : BaseCardViewState
    {
        Vector3 DefaultScale { get; set; }

        public CardViewDraw(ICardView handler, BaseStateMachine fsm, CardViewParameters parameters) : base(handler, fsm, parameters) { }

        public override void OnEnterState()
        {
            DefaultScale = Handler.Transform.localScale;
            Handler.Transform.localScale *= Parameters.StartSizeWhenDraw;
            Handler.ScaleTo(DefaultScale, Parameters.ScaleSpeed);

            Handler.Scale.OnFinishMotion += GoToIdle;
        }

        public override void OnExitState() => Handler.Scale.OnFinishMotion -= GoToIdle;

        #region Utils

        private void GoToIdle()
        {
            FSM.PopState(true);
            FSM.PushState<CardViewIdle>();
        }

        #endregion
    }
}