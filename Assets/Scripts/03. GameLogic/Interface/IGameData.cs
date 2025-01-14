using System.Collections.Generic;

namespace Marsion
{
    public interface IGameData
    {
        Dictionary<ulong, IPlayer> Players { get; }
        IPlayer CurrentPlayer { get; set; }
        int TurnCount { get; }
        void Init(IGameLogicConfig config);
        void SetPlayer(IPlayer player);
        void SetCurrentPlayer(ulong playerID);
        IPlayer GetPlayer(ulong PlayerID);
        ICard GetHandCard(ulong playerID, string cardUID);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
        ICard GetFieldCard(ulong playerID, string attackerUID);
    }
}