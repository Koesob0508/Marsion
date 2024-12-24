using System.Collections.Generic;

namespace Marsion
{
    public interface IGameData
    {
        Player[] Players { get; }
        Player CurrentPlayer { get; set; }
        int TurnCount { get; }
        Player GetPlayer(ulong PlayerID);
        void Init(IGameLogicConfig config, List<PlayerInfo> playerInfos);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
    }
}