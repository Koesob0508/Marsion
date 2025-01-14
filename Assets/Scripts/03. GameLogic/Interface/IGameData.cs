using System.Collections.Generic;

namespace Marsion
{
    public interface IGameData
    {
        Dictionary<ulong, DefaultPlayer> Players { get; }
        DefaultPlayer CurrentPlayer { get; set; }
        int TurnCount { get; }
        void Init(IGameLogicConfig config);
        void SetPlayer(DefaultPlayer player);
        void SetCurrentPlayer(ulong playerID);
        DefaultPlayer GetPlayer(ulong PlayerID);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
    }
}