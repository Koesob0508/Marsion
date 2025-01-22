namespace Marsion
{
    public class EndTurnCommand : BaseCommand
    {
        #region Command Data

        private readonly ulong _commanderID;

        #endregion

        public EndTurnCommand(IGameLogic logic, ulong commanderID) : base(logic)
        {
            _commanderID = commanderID;
        }

        protected override EventData Implement()
        {
            Logic.DataHandler.ChangeCurrentPlayer();
            Logic.CommandHandler.Add(Logic.CommandFactory.CreateStartTurn(Logic.DataHandler.CurrentPlayer.ID));

            return new EventData
            {
                Type = EventType.EndTurn,
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID,
                }
            };
        }
    }
}