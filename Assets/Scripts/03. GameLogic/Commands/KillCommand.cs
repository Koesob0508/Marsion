namespace Marsion
{
    public class KillCommand : BaseCommand
    {
        #region Command Data

        private readonly ulong _commanderID;
        private readonly string _commanderCardUID;
        private readonly ulong _targetPlayerID;
        private readonly string _targetCardUID;

        #endregion

        public KillCommand(IGameLogic logic, ulong commanderID, string commanderCardUID, ulong targetPlayerID, string targetCardUID) : base(logic)
        {
            _commanderID = commanderID;
            _commanderCardUID = commanderCardUID;
            _targetPlayerID = targetPlayerID;
            _targetCardUID = targetCardUID;
        }

        protected override EventData Implement()
        {
            // 해당 타겟 카드가 해당 플레이어 Field에 있는지
            Logic.DataHandler.TryGetCardFromField(_targetPlayerID, _targetCardUID, out var card);

            if (card != null)
            {
                card.Kill();
                Logger.Log<KillCommand>($"Player {_targetPlayerID} card {_targetCardUID} killed");
            }

            return new EventData
            {
                Type = EventType.Kill,
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID,
                    CardUID = _commanderCardUID
                },
                Target = new PlayerAndCard
                {
                    PlayerID = _targetPlayerID,
                    CardUID = _targetCardUID
                }
            };
        }
    }
}