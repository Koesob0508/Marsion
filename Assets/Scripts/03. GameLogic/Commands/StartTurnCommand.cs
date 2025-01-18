namespace Marsion
{
    public class StartTurnCommand : BaseCommand
    {
        public StartTurnCommand(IGameLogic logic) : base(logic)
        {
            EventType = EventType.StartTurn;
        }

        protected override void Implement()
        {
            var dataHandler = Logic.DataHandler;

            dataHandler.AdvanceTurn();
            
            if(dataHandler.CurrentPlayer.MaxMana < 10)
            {
                dataHandler.CurrentPlayer.IncreaseMaxMana(1);
            }

            dataHandler.CurrentPlayer.RestoreAllMana();

            Logic.CommandHandler.Add(Logic.CommandFactory.CreateDraw(dataHandler.CurrentPlayer.PlayerID, 1));
        }
    }
}