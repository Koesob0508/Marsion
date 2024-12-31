using System.Collections.Generic;

namespace Marsion
{
    public interface IGameData
    {
        Dictionary<ulong, Player> Players { get; }
        Player CurrentPlayer { get; set; }
        int TurnCount { get; }
        void Init(IGameLogicConfig config);
        void SetPlayer(Player player);
        void SetCurrentPlayer(ulong playerID);
        Player GetPlayer(ulong PlayerID);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
    }
}