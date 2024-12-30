using System.Collections.Generic;

namespace Marsion
{
    public interface IGameData
    {
        Player[] Players { get; }
        Player CurrentPlayer { get; set; }
        int TurnCount { get; }
        void Init(IGameLogicConfig config);
        void SetPlayer(int index, Player player);
        void SetCurrentPlayer(ulong index);
        Player GetPlayer(ulong PlayerID);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
    }
}