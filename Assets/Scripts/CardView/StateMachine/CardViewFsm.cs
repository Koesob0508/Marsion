namespace Marsion
{
    public class CardViewFsm : BaseStateMachine
    {
        #region Fields & Properties

        private CardViewParameters Parameters;

        private CardViewDraw DrawState { get; }
        private CardViewIdle IdleState { get; }
        private CardViewHover HoverState { get; }
        private CardViewSelect SelectState { get; }
        private CardViewDrag DragState { get; }

        #endregion

        #region Constructor

        // ICardView는 IFsmHandler를 받고 있기 때문에 Handler로 동작할 수 있다.
        // BaseStateMachine을 건들지 않고 IFsmHandler -> ICardView 확장
        public CardViewFsm(CardViewParameters parameters, ICardView handler = null) : base(handler)
        {
            Parameters = parameters;

            DrawState = new CardViewDraw(handler, this, parameters);
            IdleState = new CardViewIdle(handler, this, parameters);
            HoverState = new CardViewHover(handler, this, parameters);
            SelectState = new CardViewSelect(handler, this, parameters);
            DragState = new CardViewDrag(handler, this, parameters);

            RegisterState(DrawState);
            RegisterState(IdleState);
            RegisterState(HoverState);
            RegisterState(SelectState);
            RegisterState(DragState);

            Initialize();
        }

        #endregion
    }
}