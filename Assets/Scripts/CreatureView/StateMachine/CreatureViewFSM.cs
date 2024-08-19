namespace Marsion.CardView
{
    public class CreatureViewFSM : BaseStateMachine
    {
        private CreatureViewSpawn SpawnState { get; }
        private CreatureViewIdle IdleState { get; }
        private CreatureViewSelect SelectState { get; }

        public CreatureViewFSM(ICreatureView handler = null) : base(handler)
        {
            SpawnState = new CreatureViewSpawn(handler, this);
            IdleState = new CreatureViewIdle(handler, this);
            SelectState = new CreatureViewSelect(handler, this);

            RegisterState(SpawnState);
            RegisterState(IdleState);
            RegisterState(SelectState);

            Initialize();
        }
    }
}