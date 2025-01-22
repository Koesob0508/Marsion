namespace Marsion
{
    public class PlayCreatureCommand : BaseCommand
    {
        #region Command Data

        private readonly ulong _commanderID;
        private readonly string _commanderCardUID;
        private readonly int _index;

        #endregion

        public PlayCreatureCommand(IGameLogic logic, ulong commanderID, string commanderCardUID, int index) : base(logic)
        {
            _commanderID = commanderID;
            _commanderCardUID= commanderCardUID;
            _index = index;
        }

        protected override EventData Implement()
        {
            Logger.Log<IGameLogic>("Try spawn card", colorName: ColorCodes.Logic);

            Logic.DataHandler.TryGetCardFromHand(_commanderID, _commanderCardUID, out var card);

            // 순서대로 작성한 후, 역순으로 Add할 것
            // 마나를 소모
            var payMana = Logic.CommandFactory.CreatePayMana(_commanderID, card.ManaCost);

            // 해당 카드 소환(손에서 낼때)
            var spawnCard = Logic.CommandFactory.CreateSpawnFromHand(_commanderID, _commanderCardUID, _index);

            // 소환 이후 처리
            var afterSpawn = Logic.CommandFactory.CreateAfterSpawn(_commanderID, _commanderCardUID);

            Logic.CommandHandler.Add(afterSpawn);
            Logic.CommandHandler.Add(spawnCard);
            Logic.CommandHandler.Add(payMana);

            return new EventData
            {
                Type = EventType.PlayCard,
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID,
                    CardUID = _commanderCardUID
                }
            };
        }
    }
}