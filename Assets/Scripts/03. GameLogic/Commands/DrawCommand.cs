namespace Marsion
{
    public class DrawCommand : BaseCommand
    {
        #region Command Data
        private readonly ulong _commanderID;
        private readonly ulong _targetID;
        private readonly int _count;
        #endregion

        public DrawCommand(IGameLogic logic, ulong commanderID, ulong targetID, int count) : base(logic)
        {
            _commanderID = commanderID;
            _targetID = targetID;
            _count = count;
        }

        protected override EventData Implement()
        {
            Logic.DataHandler.DrawCard(_targetID, out var _, count: _count);

            return new EventData()
            {
                Type = EventType.DrawCard,

                Commander = new PlayerAndCard()
                {
                    PlayerID = _commanderID,
                },

                Target = new PlayerAndCard()
                {
                    PlayerID = _targetID,
                }
            };
        }
    }
}