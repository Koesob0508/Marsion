using System.Collections.Generic;

namespace Marsion
{
    public interface IGameDataHandler
    {
        public IGameData GameData { get; }

        void Init(IGameDataHandlerFactory dataHandlerFactory);
        
        Player CurrentPlayer { get; }

        Player GetPlayer(ulong playerID);
        Player GetOpponentPlayer(ulong playerID);
        Card GetCardFromHand(ulong playerID, string cardUID);
        Card GetCardFromField(ulong playerID, string cardUID);

        void AddCardToHand(ulong playerID, Card card);
        void RemoveCardFromHand(ulong playerID, string cardUID);
        void AddCardToField(ulong playerID, Card card);
        void RemoveCardFromField(ulong playerID, string cardUID);
        void ShuffleDeck(Player player);
        void DrawCard(Player player, out Card drawnCard);
        void DrawCard(Player player, out List<Card> drawnCards, int count = 1);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
    }
}