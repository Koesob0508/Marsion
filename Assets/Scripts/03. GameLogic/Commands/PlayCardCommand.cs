using System;

namespace Marsion
{
    public class PlayCardCommand : ICommand
    {
        public event Action<GameCommandData> OnCompleted;

        private readonly IGameDataHandler _dataHandler;
        private GameCommandData _data;
        private readonly ulong playerID;
        private readonly string cardUID;
        private readonly int index;

        public PlayCardCommand(IGameDataHandler dataHandler, GameCommandData data)
        {
            _dataHandler = dataHandler;
            _data = data;
            playerID = data.PlayerID;
            cardUID = data.CardUID;
            index = data.Index;
        }

        public void Execute()
        {
            var player = _dataHandler.GetPlayer(playerID);
            var card = _dataHandler.GetCardFromHand(playerID, cardUID);

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

            _data.Success = true;
            OnCompleted?.Invoke(_data);
        }

        public void Clear()
        {
            OnCompleted = null;
        }
    }
}