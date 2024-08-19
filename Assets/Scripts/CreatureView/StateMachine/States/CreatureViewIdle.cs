namespace Marsion.CardView
{
    public class CreatureViewIdle : BaseCreatureViewState
    {
        public CreatureViewIdle(ICreatureView handler, BaseStateMachine fsm) : base(handler, fsm) { }

        public override void OnEnterState()
        {
            Managers.Logger.Log<CreatureViewIdle>("Creature Idle");
        }
    }
}