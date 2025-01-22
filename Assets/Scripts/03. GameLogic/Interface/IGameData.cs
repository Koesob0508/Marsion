using System.Collections.Generic;

namespace Marsion
{
    public interface IGameData
    {
        Dictionary<ulong, IPlayer> Players { get; }
        List<ICard> FieldCards { get; }
        IPlayer CurrentPlayer { get; set; }
        int TurnCount { get; }
        void Init(IGameLogicConfig config);
        void SetPlayer(IPlayer player);
        void SetCurrentPlayer(ulong playerID);
        IPlayer GetPlayer(ulong PlayerID);
        ICard GetHandCard(ulong playerID, string cardUID);
        void AddCardToField(ICard card);
        void RemoveCardFromField(ICard card);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
        ICard GetFieldCard(ulong playerID, string attackerUID);
    }
}