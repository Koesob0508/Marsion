using System.Collections.Generic;

namespace Marsion
{
    public class RGameData : IGameData
    {
        public Dictionary<ulong, IPlayer> Players { get; private set; }

        public IPlayer CurrentPlayer { get; set; }

        public int TurnCount { get; private set; }

        public void Init(IGameLogicConfig config)
        {
            Players = new();
        }

        public void AdvanceTurn()
        {
            throw new System.NotImplementedException();
        }

        public void ChangeCurrentPlayer()
        {
            throw new System.NotImplementedException();
        }

        public ICard GetFieldCard(ulong playerID, string attackerUID)
        {
            throw new System.NotImplementedException();
        }

        public ICard GetHandCard(ulong playerID, string cardUID)
        {
            throw new System.NotImplementedException();
        }

        public IPlayer GetPlayer(ulong PlayerID)
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentPlayer(ulong playerID)
        {
            throw new System.NotImplementedException();
        }

        public void SetPlayer(IPlayer player)
        {
            throw new System.NotImplementedException();
        }
    }
}