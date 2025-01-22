namespace Marsion
{
    public class AfterSpawnCommand : BaseCommand
    {
        #region Command Data

        private readonly ulong _commanderID;
        private readonly string _commanderCardUID;

        #endregion

        public AfterSpawnCommand(IGameLogic logic, ulong commanderID, string commanderCardUID) : base(logic)
        {
            _commanderID = commanderID;
            _commanderCardUID = commanderCardUID;
        }

        protected override EventData Implement()
        {
            Logic.DataHandler.TryGetCardFromField(_commanderID, _commanderCardUID, out var card);

            card.RegisterAllTriggerAbility();

            return new EventData
            {
                Type = EventType.AfterSpawn,
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID,
                    CardUID = _commanderCardUID,
                }
            };
        }
    }
}