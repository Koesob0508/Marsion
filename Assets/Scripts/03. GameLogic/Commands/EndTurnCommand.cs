namespace Marsion
{
    public class EndTurnCommand : BaseCommand
    {
        public EndTurnCommand(IGameLogic logic) : base(logic)
        {
            EventType = EventType.EndTurn;
        }

        protected override void Implement()
        {
            Logic.DataHandler.ChangeCurrentPlayer();
            Logic.CommandHandler.Add(Logic.CommandFactory.CreateStartTurn());
        }
    }
}