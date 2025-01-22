namespace Marsion
{
    public class SpawnCardFromHandCommand : BaseCommand
    {
        #region Command Data

        private readonly ulong _commanderID;
        private readonly string _commanderCardUID;
        private readonly int _index;

        #endregion

        public SpawnCardFromHandCommand(IGameLogic logic, ulong commanderID, string commanderCardUID, int index) : base(logic)
        {
            _commanderID = commanderID;
            _commanderCardUID= commanderCardUID;
            _index = index;
        }

        protected override EventData Implement()
        {
            Logger.Log<IGameLogic>($"Player {_commanderID} spawn card {_commanderCardUID} from hand.", colorName: ColorCodes.Logic);

            if(Logic.DataHandler.TryGetCardFromHand(_commanderID, _commanderCardUID, out var card))
            {
                Logic.DataHandler.RemoveCardFromHand(_commanderID, _commanderCardUID);
                Logic.DataHandler.AddCardToField(_commanderID, card, _index);

                // 전투의 함성 발동
                card.CastCompositionAbility();
            }
            else
            {
                Logger.Log<IGameLogic>($"Try spawn card from hand. But the player {_commanderID} did not have card {_commanderCardUID} in hand.", colorName: ColorCodes.Logic);
            }

            return new EventData
            {
                Type = EventType.SpawnCreatureFromHand,
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID,
                    CardUID = _commanderCardUID
                }
            };
        }
    }
}