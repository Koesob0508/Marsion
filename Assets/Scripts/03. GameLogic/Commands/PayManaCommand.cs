namespace Marsion
{
    public class PayManaCommand : ICommand
    {
        private readonly IGameLogic _gameLogic;
        private CommandData _data;

        public PayManaCommand(IGameLogic gameLogic, CommandData data)
        {
            _gameLogic = gameLogic;
            _data = data;
        }

        public void Execute()
        {
            Logger.Log<PayManaCommand>($"Player {_data.PlayerID} pay mana {_data.IntValue}", colorName: ColorCodes.Logic);

            _gameLogic.DataHandler.PayMana(_data.PlayerID, _data.IntValue);
            _gameLogic.Trigger.TriggerEvent("UpdateData", _data);
            _gameLogic.Trigger.TriggerEvent("ChangeMana", _data);
        }
    }
}