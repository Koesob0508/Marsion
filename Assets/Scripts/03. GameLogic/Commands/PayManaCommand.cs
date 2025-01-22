namespace Marsion
{
    public class PayManaCommand : BaseCommand
    {
        #region Command Data

        private readonly ulong _commanderID;
        private readonly int _amount;

        #endregion 

        public PayManaCommand(IGameLogic logic, ulong commanderID, int amount) : base(logic)
        {
            _commanderID = commanderID;
            _amount = amount;
        }

        protected override EventData Implement()
        {
            Logger.Log<IGameLogic>($"Player {_commanderID} pay mana {_amount}", colorName: ColorCodes.Logic);

            Logic.DataHandler.PayMana(_commanderID, _amount);

            return new EventData
            {
                Type = EventType.PayMana,
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID
                }
            };
        }
    }
}