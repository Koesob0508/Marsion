using System;

namespace Marsion
{
    public class PlayCardCommand : ICommand
    {
        private readonly IGameLogic _gameLogic;
        private CommandData _data;

        public PlayCardCommand(IGameLogic gameLogic, CommandData data)
        {
            _gameLogic = gameLogic;
            _data = data;
        }

        public void Execute()
        {
            var player = _gameLogic.DataHandler.GetPlayer(_data.PlayerID);
            var card = _gameLogic.DataHandler.GetCardFromHand(_data.PlayerID, _data.CardUID);
            
            Logger.Log<DefaultGameLogic>($"Played card : {card.Name} at position {_data.IntValue}", colorName: ColorCodes.Logic);

            _gameLogic.DataHandler.RemoveCardFromHand(_data.PlayerID, _data.CardUID);
            _gameLogic.DataHandler.AddCardToField(_data.PlayerID, card, _data.IntValue);
            card.ExecutePlayAbility();
            _data.Succeeded = true;

            _gameLogic.Trigger.TriggerEvent("UpdateData", _data);
            _gameLogic.Trigger.TriggerEvent("PlayCard", _data);
        }
    }
}