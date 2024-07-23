namespace Marsion
{
    public class CardViewFsm : BaseStateMachine
    {
        #region Fields & Properties

        private CardViewIdle IdleState { get; }
        private CardViewHover HoverState { get; }
        private CardViewSelect SelectState { get; }
        private CardViewDrag DragState { get; }

        #endregion

        #region Constructor

        // ICardView는 IFsmHandler를 받고 있기 때문에 Handler로 동작할 수 있다.
        // BaseStateMachine을 건들지 않고 IFsmHandler -> ICardView 확장
        public CardViewFsm(ICardView handler = null) : base(handler)
        {
            IdleState = new CardViewIdle(handler, this);
            HoverState = new CardViewHover(handler, this);
            SelectState = new CardViewSelect(handler, this);
            DragState = new CardViewDrag(handler, this);

            RegisterState(IdleState);
            RegisterState(HoverState);
            RegisterState(SelectState);
            RegisterState(DragState);

            Initialize();
        }

        #endregion
    }
}