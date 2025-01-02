using System;

namespace Marsion
{
    public class PlayCardCommand : ICommand
    {
        private readonly IGameLogic _gameLogic;
        private GameCommandData _data;
        private readonly ulong playerID;
        private readonly string cardUID;
        private readonly int index;

        public PlayCardCommand(IGameLogic gameLogic, GameCommandData data)
        {
            _gameLogic = gameLogic;
            _data = data;
            playerID = data.PlayerID;
            cardUID = data.CardUID;
            index = data.Index;
        }

        public void Execute()
        {
            var player = _gameLogic.DataHandler.GetPlayer(playerID);
            var card = _gameLogic.DataHandler.GetCardFromHand(playerID, cardUID);

            if(player.Mana < card.ManaCost)
            {
                Logger.Log<DefaultGameLogic>($"Not enoufh mana to play {card.Name}", colorName: ColorCodes.Logic);
                return;
            }

            player.PayMana(card.ManaCost);
            player.Hand.Remove(card);
            player.Field.Insert(index, card);
            card.ExecutePlayAbility();

            Logger.Log<DefaultGameLogic>($"Played card : {card.Name} at position {index}", colorName: ColorCodes.Logic);

            _data.Succeeded = true;

            _gameLogic.EventHandler.TriggerEvent("CardPlayed", _data);
        }
    }
}