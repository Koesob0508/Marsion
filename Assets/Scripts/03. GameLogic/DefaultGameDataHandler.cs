using System.Collections.Generic;

namespace Marsion
{
    public class DefaultGameDataHandler : IGameDataHandler
    {
        public IGameData GameData { get; private set; }

        public DefaultGameDataHandler(IGameData gameData)
        {
            GameData = gameData;
        }

        public void AddCardToField(ulong playerID, Card card)
        {
            throw new System.NotImplementedException();
        }

        public void AddCardToHand(ulong playerID, Card card)
        {
            throw new System.NotImplementedException();
        }

        public Card GetCardFromField(ulong playerID, string cardUID)
        {
            throw new System.NotImplementedException();
        }

        public Card GetCardFromHand(ulong playerID, string cardUID)
        {
            throw new System.NotImplementedException();
        }

        public Player GetOpponentPlayer(ulong playerID)
        {
            throw new System.NotImplementedException();
        }

        public Player GetPlayer(ulong playerID)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveCardFromField(ulong playerID, string cardUID)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveCardFromHand(ulong playerID, string cardUID)
        {
            throw new System.NotImplementedException();
        }

        public void SetPlayerDeck(ulong playerID, List<string> deck)
        {
            throw new System.NotImplementedException();
        }
    }
}
