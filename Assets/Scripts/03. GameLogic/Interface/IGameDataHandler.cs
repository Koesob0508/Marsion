using System.Collections.Generic;

namespace Marsion
{
    public interface IGameDataHandler
    {
        public IGameData GameData { get; }

        void Init(IGameDataHandlerFactory dataHandlerFactory);
        
        IPlayer CurrentPlayer { get; }

        bool TryGetPlayer(ulong playerID, out IPlayer player);
        ulong GetOpponentPlayerID(ulong playerID);
        bool TryGetCardFromHand(ulong playerID, string cardUID, out ICard card);
        bool TryGetCardFromField(ulong playerID, string cardUID, out ICard card);

        void PayMana(ulong playerID, int amount);
        void AddCardToHand(ulong playerID, ICard card);
        void RemoveCardFromHand(ulong playerID, string cardUID);
        void AddCardToField(ulong playerID, ICard card, int index);
        void RemoveCardFromField(ulong playerID, string cardUID);
        void ShuffleDeck(ulong playerID);
        void DrawCard(ulong playerID, out ICard drawnCard);
        void DrawCard(ulong playerID, out List<ICard> drawnCards, int count = 1);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
    }
}