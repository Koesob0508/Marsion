namespace Marsion
{
    public class StartGameCommand : BaseCommand
    {
        public StartGameCommand(IGameLogic logic) : base(logic)
        {
            EventType = EventType.StartGame;
        }

        protected override void Implement()
        {
            var dataHandler = Logic.DataHandler;

            Logic.CommandHandler.Add(Logic.CommandFactory.CreateStartTurn());
            Logic.CommandHandler.Add(Logic.CommandFactory.CreateDraw(dataHandler.CurrentPlayer.PlayerID, 3));
            Logic.CommandHandler.Add(Logic.CommandFactory.CreateDraw(dataHandler.GetOpponentPlayerID(dataHandler.CurrentPlayer.PlayerID), 4));
        }
    }
}