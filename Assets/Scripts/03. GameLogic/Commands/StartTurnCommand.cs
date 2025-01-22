namespace Marsion
{
    public class StartTurnCommand : BaseCommand
    {
        #region CommandData
        private readonly ulong _commanderID;
        #endregion

        public StartTurnCommand(IGameLogic logic, ulong commanderID) : base(logic)
        {
            _commanderID = commanderID;
        }

        protected override EventData Implement()
        {
            var dataHandler = Logic.DataHandler;

            dataHandler.AdvanceTurn();
            
            if(dataHandler.CurrentPlayer.MaxMana < 10)
            {
                dataHandler.CurrentPlayer.IncreaseMaxMana(1);
            }

            dataHandler.CurrentPlayer.RestoreAllMana();
            var currentPlayerID = dataHandler.CurrentPlayer.ID;
            Logic.CommandHandler.Add(Logic.CommandFactory.CreateDraw(currentPlayerID, currentPlayerID, 1));

            return new EventData
            {
                Type = EventType.StartTurn,
                Commander = new PlayerAndCard
                {
                    PlayerID = currentPlayerID,
                }
            };
        }
    }
}