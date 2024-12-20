namespace Marsion
{
    public interface IGameData
    {
        Player[] Players { get; }
        Player CurrentPlayer { get; set; }
        int TurnCount { get; }
        Player GetPlayer(ulong PlayerID);
        void Init(IGameDataConfig config);
        void AdvanceTurn();
        void ChangeCurrentPlayer();
    }
}