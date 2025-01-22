namespace Marsion
{
    public class StartGameCommand : BaseCommand
    {
        public StartGameCommand(IGameLogic logic) : base(logic) { }

        protected override EventData Implement()
        {
            var dataHandler = Logic.DataHandler;
            var currentPlayerID = dataHandler.CurrentPlayer.ID;
            var opponentPlayerID = dataHandler.GetOpponentPlayerID(currentPlayerID);

            Logic.CommandHandler.Add(Logic.CommandFactory.CreateStartTurn(currentPlayerID));
            Logic.CommandHandler.Add(Logic.CommandFactory.CreateDraw(currentPlayerID, currentPlayerID, 3));
            Logic.CommandHandler.Add(Logic.CommandFactory.CreateDraw(opponentPlayerID, opponentPlayerID, 4));

            return new EventData { Type = EventType.StartGame };
        }
    }
}